# Jwt Refresh Token
.NET library to allowing a client application get new access tokens

### Introduction

Jwt Refresh Token is .Net library to provide a importante authentication aspects, and using Jwt Token (learn more [jwt.io](https://jwt.io) web site) 
and **Refresh Token**.

**Refresh Token** basically require a unique token identifier to obtain additional access tokens. Access token arent'n valid for an long period for security 
and **Refresh Token** strategy can help to re-authentication a user without login credential 🤔 (some scratches risks here)

### Architecture
This project is based in Onion Architecture created by [Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/) in 2008.

* [Jwt.Refresh.Token.Infrastructure]()
* [Jwt.Refresh.Token.Application]()
* [Jwt.Refresh.Token.Domain]()
* [Jwt.Refresh.Token.Tests]()

#### Flow
![Miro](https://i.imgur.com/f8y4CGR.jpg)

#### Supported Databases
Goal of this project is support at last 3 main Azure Databases:
- [x] [Azure Cosmos DB for NoSQL](https://learn.microsoft.com/en-gb/azure/cosmos-db/nosql/quickstart-dotnet?tabs=azure-portal%2Cwindows%2Cpasswordless%2Csign-in-azure-cli) *preview*
- [ ] [Azure Cosmos DB for PostgreSQL](https://learn.microsoft.com/en-gb/azure/cosmos-db/postgresql/introduction) *not started*
- [ ] [Azure Sql Database](https://azure.microsoft.com/en-us/products/azure-sql/database/?&ef_id=CjwKCAjwiOCgBhAgEiwAjv5whFE2R0wjiJxJRIQlHjt35KZpzb_JowGvDnAvkdSRvg5VbBaeMBlmZhoCkP0QAvD_BwE:G:s&OCID=AIDcmmzmnb0182_SEM_CjwKCAjwiOCgBhAgEiwAjv5whFE2R0wjiJxJRIQlHjt35KZpzb_JowGvDnAvkdSRvg5VbBaeMBlmZhoCkP0QAvD_BwE:G:s&gclid=CjwKCAjwiOCgBhAgEiwAjv5whFE2R0wjiJxJRIQlHjt35KZpzb_JowGvDnAvkdSRvg5VbBaeMBlmZhoCkP0QAvD_BwE) *not started*

#### Cosmos DB
To install Jwt.Refresh.Token.Cosmos *(include prereleases)*, run the following command in the [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)
```
dotnet add package Jwt.Refresh.Token.Cosmos --prerelease
```
##### Usage

1. ✯ Cosmos setup

The first step to do is provision your cosmos db, or if you already have it, create the token container id (*choose name you want*) with partitionKey **'/userId'**.

User container is optional. If you want control user data with Jwt Refresh Token library, you need create user container but if you already have other
user data control, just implement IUserRepository for get user by id and password.

2. ✯ Configure settings app:
```json
"JwtRefreshTokenDescriptor": {
    "AlgorithmKey": "YOUR_ALGORITHM_KEY",
    "Issuer": "https://your-resource.com",
    "Audience": "https://your-resource.com"
  },
  "JwtRefreshTokenExpires": {
    "CreateMilliseconds": 60000,
    "RefreshMilliseconds": 900000
  },
  "JwtRefreshTokenCosmos": {
    "ConnectionString": "YOUR_COSMOS_CONNECTIONSTRING",
    "DatabaseId": "YOUR_DATABASEID",
    "UserContainerId": "YOUR_USER_CONTAINERID",
    "TokenContainerId": "YOUR_TOKEN_CONTAINERID"
  }
```

3. ✯ Configure startup app:

Install ASP.NET Core authentication middleware
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```
and configuring it in your application’s startup class file: 

```csharp
// [required] Add jwt domain services
builder.Services.AddJwtRefreshTokenServices(builder.Configuration);

// [required (cosmos)]  Add jwt cosmos repositories
builder.Services.AddJwtRefreshTokenCosmosServices(builder.Configuration);

// [optional]  Bind util token expires config
builder.Services.BindJwtRefreshTokenExpiresOptions(builder.Configuration);

// [required] AspNetCore Authentication config
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // choose your bearer config 
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = true;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
              .GetBytes(builder.Configuration.GetValue<string>("JwtRefreshTokenDescriptor:AlgorithmKey"))),
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

// [required] AspNetCore Authentication config
builder.Services
    .AddAuthorization(auth =>
    {
        auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());
    });
```

4. ✯ Create token controller

Creating token controler to management token:

```csharp
[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenAppService _tokenAppService;
    private readonly IOptionsSnapshot<JwtRefreshTokenExpiresOptions> _jwtRefreshTokenExpiresOptions;

    public TokenController(ILogger<TokenController> logger, 
        ITokenAppService tokenAppService,
        IOptionsSnapshot<JwtRefreshTokenExpiresOptions> jwtRefreshTokenExpiresOptions)
    {
        _logger = logger;
        _tokenAppService = tokenAppService;
        _jwtRefreshTokenExpiresOptions = jwtRefreshTokenExpiresOptions;
    }

    private string GetRemoteIpAddress()
    {
        return this.Request?.HttpContext?.Connection?
            .RemoteIpAddress?.ToString();
    }

    [HttpPost("")]
    public async Task<IActionResult> PostAsync([FromForm] string userId, 
      [FromForm] string password, CancellationToken cancellationToken)
    {
        var token = await _tokenAppService.CreateAsync(userId, 
            password, _jwtRefreshTokenExpiresOptions.Value.CreateMilliseconds,
            GetRemoteIpAddress(), cancellationToken);

        return new TokenResult(token);
    }

    [Authorize("Bearer")]
    [HttpPatch("")]
    public async Task<IActionResult> RefreshAsync([FromForm] string tokenId,
      [FromForm] string userId, CancellationToken cancellationToken)
    {
        var token = await _tokenAppService.RefreshAsync(tokenId, 
            userId, _jwtRefreshTokenExpiresOptions.Value.RefreshMilliseconds,
            GetRemoteIpAddress(), cancellationToken);

        return new TokenResult(token);
    }

    [Authorize("Bearer")]
    [HttpPatch("/revoke")]
    public async Task<IActionResult> RevokeAsync([FromForm] string tokenId,
      [FromForm] string userId, CancellationToken cancellationToken)
    {
        var updated = await _tokenAppService.TryRevokeAsync(tokenId, 
            userId, GetRemoteIpAddress(), cancellationToken);

        return Ok(new { updated = updated });
    }
}
```

See integration test api [here](https://github.com/brunobrandes/jwt-refresh-token/tree/main/src/Tests/Jwt.Refresh.Token.Tests.Integrations.Api)

### TODO

- [ ] Incrise unit test coverage
- [ ] Create pipeline
- [ ] Implement PostgreeSql infrastructure
- [ ] Implement Sql Databse infrastructure
