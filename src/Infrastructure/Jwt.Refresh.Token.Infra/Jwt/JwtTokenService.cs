using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Refresh.Token.Domain.Configurations;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Jwt.Refresh.Token.Infra.Jwt
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IClaimsIdentityService _claimsIdentityService;
        private readonly IOptionsSnapshot<JwtRefreshTokenDescriptorOptions> _jwtRefreshTokenDescriptorOptions;
        public JwtTokenService(
            IClaimsIdentityService claimService,
            IOptionsSnapshot<JwtRefreshTokenDescriptorOptions> jwtRefreshTokenDescriptorOptions)
        {
            _claimsIdentityService = claimService;
            _jwtRefreshTokenDescriptorOptions = jwtRefreshTokenDescriptorOptions;

        }

        public async Task<string> GetAccessTokenAsync(string userId, int expiresMilliSeconds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = await _claimsIdentityService.GetAsync(userId),
                Expires = DateTime.UtcNow.AddMilliseconds(expiresMilliSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_jwtRefreshTokenDescriptorOptions.Value.AlgorithmKey)),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtRefreshTokenDescriptorOptions.Value.Issuer,
                IssuedAt = !string.IsNullOrEmpty(_jwtRefreshTokenDescriptorOptions.Value.Issuer) ? DateTime.UtcNow : null,
                Audience = _jwtRefreshTokenDescriptorOptions.Value.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

