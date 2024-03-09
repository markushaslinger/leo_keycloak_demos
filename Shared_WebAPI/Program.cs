using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

const string CorsPolicyName = "AllowOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
       {
           options.MetadataAddress = "https://auth.htl-leonding.ac.at/realms/htl-leonding/.well-known/openid-configuration";
           options.Authority = "https://auth.htl-leonding.ac.at/realms/htl-leonding";
           options.Audience = "htlleonding-service";
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               ValidateIssuer = true,
               // currently the audience is not set by KeyCloak for access_token - turn off if not using id_token
               ValidateAudience = true,
               // currently token issued from KeyCloak expire after 1 minute - turn on once fixed
               ValidateLifetime = false,
               ClockSkew = TimeSpan.FromMinutes(1),
           };
           options.RequireHttpsMetadata = false;
       });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DemoAuth Backend",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, []
        }
    });
});
builder.Services.AddCors(o => o.AddPolicy(CorsPolicyName, b =>
{
    // TODO adjust origin if your client is not running on localhost:5005
    // origin has to be set if credentials are used (which we need)
    const string Origin = "https://localhost:5005";
    
    // this is _not_ a production configuration!
    b.WithOrigins(Origin)
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicyName);

// when not using a reverse proxy (e.g. nginx) - which you should - uncomment the following line
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
