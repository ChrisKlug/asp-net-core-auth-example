# Example of ASP.NET Core auth "issue" during testing

This repo contains some sample code to try and explain a "problem" that I have with the ASP.NET Core authentication system when it comes to integration testing

## Problem?

Well, it isn't really a problem I guess, now that I have been told that it can be solved by the following code

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