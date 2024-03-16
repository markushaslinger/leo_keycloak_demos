using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorClient;
using BlazorClient.Core;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
    var providerOptions = options.ProviderOptions;
    providerOptions.MetadataUrl
        = "https://auth.htl-leonding.ac.at/realms/htl-leonding/.well-known/openid-configuration";
    providerOptions.Authority = "https://auth.htl-leonding.ac.at/realms/htl-leonding";
    providerOptions.ResponseType = "id_token token";
    providerOptions.ClientId = "htlleonding-service";
});
builder.Services.ConfigureHttpClient();

builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
