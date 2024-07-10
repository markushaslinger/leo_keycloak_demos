using LeoUserInfo.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
       {
           options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
           options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
       })
       // SSR => user session exists and id is stored in a cookie
       .AddCookie()
       .AddOpenIdConnect(options =>
       {
           options.Authority = "https://auth.htl-leonding.ac.at/realms/htlleonding/";
           options.ClientId = "htlleonding-service";
           // TODO: set client secret
           options.ClientSecret = "TODO_SET_CLIENT_SECRET";
           options.ResponseType = OpenIdConnectResponseType.Code;
           options.SaveTokens = true;
           options.Scope.Add("openid");
       });
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddRazorComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// if not using a reverse proxy (e.g. nginx) - which you should do, uncomment this line
//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
// once authorization (e.g. roles) is set up, uncomment this line
// app.UseAuthorization();

app.MapRazorComponents<App>();

app.Run();
