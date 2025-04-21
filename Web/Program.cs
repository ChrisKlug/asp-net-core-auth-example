using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddOpenIdConnect(options =>
{
    options.Authority = "https://non-existent-url-during_testing";
})
.AddCookie();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/secure", () => "Hello World!").RequireAuthorization();

app.Run();

public partial class Program { };