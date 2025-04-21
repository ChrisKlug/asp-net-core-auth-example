using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Tests;

public class IntegrationTests
{
    [Fact]
    public async Task Does_not_work_because_of_OIDC_config_call()
    {
        var app = new WebApplicationFactory<Program>();

        var client  = app.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Does_not_work_because_of_OIDC_config_call_2()
    {
        var app = new WebApplicationFactory<Program>()
                        .WithWebHostBuilder(builder =>
                        {
                            builder.ConfigureTestServices(services =>
                            {
                                services.AddAuthentication(options =>
                                {
                                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                    options.DefaultChallengeScheme = BasicAuthenticationDefaults.AuthenticationScheme;
                                })
                                .AddBasicAuthentication(creds =>
                                    Task.FromResult(creds.username.Equals("Test", StringComparison.InvariantCultureIgnoreCase)
                                                    && creds.password == "test"));
                            });
                        });

        var client = app.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Does_not_work_either_because_of_OIDC_config_call()
    {
        var app = new WebApplicationFactory<Program>()
                        .WithWebHostBuilder(builder =>
                        {
                            builder.ConfigureTestServices(services =>
                            {
                                services.AddAuthentication(options =>
                                {
                                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                    options.DefaultChallengeScheme = BasicAuthenticationDefaults.AuthenticationScheme;
                                })
                                .AddBasicAuthentication(creds => 
                                    Task.FromResult(creds.username.Equals("Test", StringComparison.InvariantCultureIgnoreCase) 
                                                    && creds.password == "test"));
                            });
                        });

        var client = app.CreateClient();

        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("Test:test")));

        var response = await client.GetAsync("/secure");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Works_but_it_would_be_nice_to_be_able_to_handle_OIDC_removal_in_ConfigureTestServices()
    {
        var app = new WebApplicationFactory<Program>()
                        .WithWebHostBuilder(builder =>
                        {
                            builder.ConfigureTestServices(services =>
                            {
                                services.AddAuthentication(options =>
                                {
                                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                    options.DefaultChallengeScheme = BasicAuthenticationDefaults.AuthenticationScheme;
                                })
                                .AddBasicAuthentication(creds =>
                                    Task.FromResult(creds.username.Equals("Test", StringComparison.InvariantCultureIgnoreCase)
                                                    && creds.password == "test"));
                            });
                        });

        app.Services.GetRequiredService<IAuthenticationSchemeProvider>()
                    .RemoveScheme(OpenIdConnectDefaults.AuthenticationScheme);

        var client = app.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task What_I_would_really_like_to_have()
    {
        var app = new WebApplicationFactory<Program>()
                        .WithWebHostBuilder(builder =>
                        {
                            builder.ConfigureTestServices(services =>
                            {
                                services.AddAuthentication(options =>
                                {
                                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                    options.DefaultChallengeScheme = BasicAuthenticationDefaults.AuthenticationScheme;
                                })
                                .RemoveScheme(OpenIdConnectDefaults.AuthenticationScheme)
                                .AddBasicAuthentication(creds =>
                                    Task.FromResult(creds.username.Equals("Test", StringComparison.InvariantCultureIgnoreCase)
                                                    && creds.password == "test"));
                            });
                        });

        var client = app.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public static class DummyAuthenticationBuilderExtensions
{
    public static AuthenticationBuilder RemoveScheme(this AuthenticationBuilder builder, string schemeName)
    {
        // TODO: Implement
        return builder;
    }
}
