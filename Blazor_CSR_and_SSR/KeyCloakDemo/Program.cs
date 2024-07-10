using KeyCloakDemo.Client;
using KeyCloakDemo.Client.Pages;
using KeyCloakDemo.Components;
using LeoAuth.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents()
       .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingLeoAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
       {
           options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
       })
       .AddCookie(options =>
       {
           options.LoginPath = "/auth/login";
           options.LogoutPath = "/auth/logout";
       })
       .AddOpenIdConnect(options =>
       {
           options.Authority = "https://auth.htl-leonding.ac.at/realms/htlleonding/";
           options.ClientId = "htlleonding-service";
           // TODO: set client secret
           options.ClientSecret = "TODO_SET_CLIENT_SECRET";
           options.ResponseType = "code";
           options.CallbackPath = "/signin-oidc";
           options.SignedOutCallbackPath = "/signout-callback-oidc";
           options.SaveTokens = true;
           options.Scope.Add("openid");
       });

builder.Services.AddHttpContextAccessor();

builder.Services
       .AddTransient<CookieHandler>()
       .AddScoped(sp => sp
                        .GetRequiredService<IHttpClientFactory>()
                        .CreateClient("API"))
       .AddHttpClient("API", client => client.BaseAddress = new Uri("http://localhost:5173"))
       .AddHttpMessageHandler<CookieHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/auth/login", async context =>
{
    var authProperties = new AuthenticationProperties
    {
        RedirectUri = "/"
    };
    await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, authProperties);
});

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
