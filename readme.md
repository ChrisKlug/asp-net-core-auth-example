# Example of ASP.NET Core auth "issue" during testing

This repo contains some sample code to try and explain a "problem" that I have with the ASP.NET Core authentication system when it comes to integration testing

## Problem

The issue is that I would expect it to be possible to remove an authentication handler in `ConfigureTestServices`, before the app was actually created. Something like

```csharp
var app = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // Like this
                        services.AddAuthentication(options => {
                              // ...
                           })
                           .RemoveScheme(OpenIdConnectDefaults.AuthenticationScheme)
                           .AddBasicAuthentication(creds => ...);	
                    });
                });
```

In honesty, it isn't really a problem now that I have been told that it can be solved by the following code

```csharp
var app = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // Removed for simplicity
                    });
                });

app.Services.GetRequiredService<IAuthenticationSchemeProvider>()
            .RemoveScheme(OpenIdConnectDefaults.AuthenticationScheme);
```

However, I still feel lite it would make more sense to remove the handler already in the `ConfigureTestServices()` method, as this is where I would expect to do it...