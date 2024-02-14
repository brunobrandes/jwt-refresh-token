using Azure.Core;
using Jwt.Refresh.Token.Application;
using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Configurations;
using Jwt.Refresh.Token.Infra.Cosmos.Configurations;
using Jwt.Refresh.Token.Infra.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Jwt.Refresh.Token.Tests.Integrations.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenAppService _tokenAppService;
    private readonly IOptionsSnapshot<JwtRefreshTokenExpiresOptions> _jwtRefreshTokenExpiresOptions;

    public TokenController(ILogger<TokenController> logger, ITokenAppService tokenAppService,
        IOptionsSnapshot<JwtRefreshTokenExpiresOptions> jwtRefreshTokenExpiresOptions)
    {
        _logger = logger;
        _tokenAppService = tokenAppService;
        _jwtRefreshTokenExpiresOptions = jwtRefreshTokenExpiresOptions;
    }

    private string GetRemoteIpAddress()
    {
        return Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    [HttpPost("")]
    public async Task<IActionResult> PostAsync([FromForm] string userId, [FromForm] string password, CancellationToken cancellationToken)
    {
        var token = await _tokenAppService.CreateAsync(userId, password, _jwtRefreshTokenExpiresOptions.Value.CreateMilliseconds,
            GetRemoteIpAddress(), cancellationToken);

        return new TokenResult(token);
    }

    [Authorize("Bearer")]
    [HttpPatch("")]
    public async Task<IActionResult> RefreshAsync([FromForm] string tokenId, [FromForm] string userId, CancellationToken cancellationToken)
    {
        var token = await _tokenAppService.RefreshAsync(tokenId, userId, _jwtRefreshTokenExpiresOptions.Value.RefreshMilliseconds,
            GetRemoteIpAddress(), cancellationToken);

        return new TokenResult(token);
    }

    [Authorize("Bearer")]
    [HttpPatch("/revoke")]
    public async Task<IActionResult> RevokeAsync([FromForm] string tokenId, [FromForm] string userId, CancellationToken cancellationToken)
    {
        var updated = await _tokenAppService.TryRevokeAsync(tokenId, userId, GetRemoteIpAddress(), cancellationToken);
        return Ok(new { updated = updated });
    }
}

