using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using OneOf;
using OneOf.Types;

namespace BlazorClient.Core;

public interface IBackendService
{
    ValueTask<OneOf<Success<string>, Error>> CheckLoggedIn();
    ValueTask<OneOf<Success<string>, Error>> CheckAtLeastStudent();
    ValueTask<OneOf<Success<string>, Error>> CheckIsTeacher();
    ValueTask<OneOf<Success<string>, Error>> CheckAnonymous();
    ValueTask<OneOf<Success<string[]>, Error>> GetTokenData();
}

public sealed class BackendService(IHttpClientFactory client, IAccessTokenProvider tokenProvider) : IBackendService
{
    internal const string ClientName = "WebAPI";
    private IHttpClientFactory HttpClientFactory { get; } = client;
    private IAccessTokenProvider TokenProvider { get; } = tokenProvider;
    
    // Note: exception handling here is very sloppy (just so the demos don't crash the app)
    // don't do that for real (instead log, return meaningful errors, etc.)

    public ValueTask<OneOf<Success<string>, Error>> CheckLoggedIn() => PerformCheckCall("/at-least-logged-in");
    public ValueTask<OneOf<Success<string>, Error>> CheckAtLeastStudent() => PerformCheckCall("/at-least-student");
    public ValueTask<OneOf<Success<string>, Error>> CheckIsTeacher() => PerformCheckCall("/is-teacher");

    public async ValueTask<OneOf<Success<string>, Error>> CheckAnonymous()
    {
        try
        {
            var client = await GetClient();
            // set the authorization header to anonymous for this test
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Anonymous");

            var response = await client.GetAsync("/everyone-allowed");
            if (!response.IsSuccessStatusCode)
            {
                return new Error();
            }

            var content = await response.Content.ReadAsStringAsync();

            return new Success<string>(content);
        }
        catch (Exception)
        {
            return new Error();
        }
    }

    public async ValueTask<OneOf<Success<string[]>, Error>> GetTokenData()
    {
        try
        {
            var client = await GetClient();
            var response = await client.GetAsync("/token-data");
            if (!response.IsSuccessStatusCode)
            {
                return new Error();
            }

            var content = await response.Content.ReadFromJsonAsync<string[]>();

            return new Success<string[]>(content ?? []);
        } 
        catch (Exception)
        {
            return new Error();
        }
    }

    private async ValueTask<OneOf<Success<string>, Error>> PerformCheckCall(string route)
    {
        try
        {
            var client = await GetClient();
            var response = await client.GetAsync(route);
            if (!response.IsSuccessStatusCode)
            {
                return new Error();
            }

            var content = await response.Content.ReadAsStringAsync();

            return new Success<string>(content);
        }
        catch (Exception)
        {
            return new Error();
        }
    }

    private async ValueTask<HttpClient> GetClient()
    {
        var client = HttpClientFactory.CreateClient(ClientName);
        var tokenResult = await TokenProvider.RequestAccessToken();
        client.DefaultRequestHeaders.Authorization = tokenResult.TryGetToken(out var token)
            ? new AuthenticationHeaderValue("Bearer", token.Value)
            : new AuthenticationHeaderValue("Anonymous");

        return client;
    }
}

internal static class HttpClientSetup
{
    public static void ConfigureHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient(BackendService.ClientName, client =>
        {
            client.BaseAddress = new Uri("http://localhost:5050/api/demo");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.AddScoped<IBackendService, BackendService>();
    }
}
