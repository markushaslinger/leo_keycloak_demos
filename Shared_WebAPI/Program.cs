using AuthDemoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLeoAuthentication();
builder.Services.AddBasicLeoAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth();
builder.Services.AddLenientCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(Setup.CorsPolicyName);

// when not using a reverse proxy (e.g. nginx) - which you should - uncomment the following line
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
