# 11 — Configuration & Options

Every environment-specific value Atlas needs — connection strings, signing keys, SMTP credentials — flows through ASP.NET Core configuration, bound into strongly-typed options classes rather than pulled out with raw string lookups scattered through the codebase.

## The configuration shape

`appsettings.json` declares every section the app expects, with the values left blank for each environment to fill in:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Storage": {
    "BaseUrl": ""
  },
  "Jwt": {
    "Key": "",
    "Issuer": "",
    "Audience": "",
    "ExpirationInMinutes": 30,
    "RefreshTokenExpirationInDays": 30
  },
  "Email": {
    "Host": "",
    "SenderEmail": "",
    "SenderName": "Atlas Template",
    "Password": "",
    "Port": 587
  }
}
```

## What each value means, and where it comes from

| Property | Meaning | Where the value comes from |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | The SQL Server connection string `AppDbContext` uses to connect. | Your SQL Server instance's connection details — e.g. `Server=localhost;Database=AtlasTemplate;Trusted_Connection=True;TrustServerCertificate=True;` for a local instance. |
| `Jwt:Key` | The symmetric key used to sign access tokens. | Not looked up anywhere — you generate any sufficiently long, random string yourself. It just needs to stay consistent for as long as tokens signed with it should remain valid. |
| `Jwt:Issuer` / `Jwt:Audience` | Identify who issued the token and who it's intended for. | Conventionally your application's name or domain (e.g. `AtlasTemplate.Api`). They don't need to resolve to anything real — they just need to match between issuing and validating, which both happen inside this same app. |
| `Jwt:ExpirationInMinutes` / `Jwt:RefreshTokenExpirationInDays` | How long an access token and a refresh token stay valid, respectively. | A policy choice you set, not a value you look up — see [`docs/05-authentication-and-jwt.md`](05-authentication-and-jwt.md) for how each is used. |
| `Email:Host` / `Email:Port` | Your SMTP provider's server address and port. | Supplied by your SMTP provider — for Gmail, `smtp.gmail.com` on port `587`. |
| `Email:SenderEmail` / `Email:SenderName` | The address and display name mail is sent from. | The mailbox you're sending through, and whatever display name you want recipients to see. |
| `Email:Password` | Credential used to authenticate with the SMTP server. | For Gmail, an app password generated from your Google account — not your regular account password. See [`docs/09-email-system.md`](08-email-system.md) for how these are used to send mail. |
| `Storage:BaseUrl` | The base URL the running application is reachable at, used to build absolute links such as email confirmation URLs and uploaded image URLs. | Locally, whatever `launchSettings.json` has the API listening on (e.g. `https://localhost:7001`); in a deployed environment, that environment's actual public URL. |

## Getting configuration into code: the options pattern

Rather than reaching into `IConfiguration` with a string key wherever a setting is needed, each section is bound once into its own class and handed out through dependency injection. `EmailOptions` is the pattern already in place, and it's the template for how every other section is wired the same way:

```csharp
public class EmailOptions
{
    public string Host { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    public string Password { get; set; }
    public int Port { get; set; } = 587;
}
```

Binding happens once, in `Options.cs`:

```csharp
services.Configure<EmailOptions>(configuration.GetSection("Email"));
```

and from there, anything that needs it simply asks for `IOptions<EmailOptions>` in its constructor, the way `EmailService` does:

```csharp
public EmailService(IOptions<EmailOptions> emailOptions) => _emailOptions = emailOptions.Value;
```

The appeal of doing it this way rather than indexing into configuration directly is that a section's shape only exists in one place. A typo in a key name, or the wrong type for a value, surfaces at startup when the section is bound — not silently, the first time some deep call site happens to read a `null`. It also opens the door to `IOptionsSnapshot<T>` or `IOptionsMonitor<T>` later on, if configuration ever needs to reload without a full restart.
