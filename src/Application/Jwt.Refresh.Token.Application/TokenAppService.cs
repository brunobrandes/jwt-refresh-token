using System;
using Jwt.Refresh.Token.Domain.Constants;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using System.Threading.Tasks;
using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using System.Threading;
using Jwt.Refresh.Token.Domain.DataTransferObjects;
using Jwt.Refresh.Token.Domain.Enums;
using System.Net;
using Jwt.Refresh.Token.Domain.Entities;
using Jwt.Refresh.Token.Domain.Extensions;

namespace Jwt.Refresh.Token.Application
{
    public class TokenAppService : ITokenAppService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public TokenAppService(ITokenRepository tokenRepository, IUserRepository userRepository,
            IJwtTokenService jwtTokenService)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Domain.DataTransferObjects.Token> CreateAsync(string userId, string password,
            int expiresMilliseconds, string ipAddress, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                return new Domain.DataTransferObjects.Token(userId, TokenStatus.Unauthorized);

            try
            {
                var entityUserId = await _userRepository.GetUserIdByIdAndPasswordAsync(userId, password, cancellationToken);

                if (string.IsNullOrEmpty(entityUserId))
                    return new Domain.DataTransferObjects.Token(userId, TokenStatus.Unauthorized);

                var accessToken = await _jwtTokenService.GetAccessTokenAsync(userId, expiresMilliseconds, cancellationToken);

                var token = new Domain.DataTransferObjects.Token(Guid.NewGuid().ToString(), userId, accessToken,
                   TokenStatus.Authorized, expiresMilliseconds);

                await _tokenRepository.AddAsync(token.ToAuthorizedTokenEntity(ipAddress, expiresMilliseconds), cancellationToken);

                return token;
            }
            catch (Exception ex)
            {
                return new Domain.DataTransferObjects.Token(userId, TokenStatus.Error,
                    new TokenError("Unexpected error create async token", ex));
            }
        }

        public async Task<Domain.DataTransferObjects.Token> RefreshAsync(string tokenId, string userId,
            int expiresMilliseconds, string ipAddress, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(tokenId) || string.IsNullOrEmpty(userId))
                return new Domain.DataTransferObjects.Token(userId, TokenStatus.Unauthorized);

            try
            {
                var token = await _tokenRepository.GetByIdAndUserIdAsync(tokenId, userId);

                if (token is null)
                    return new Domain.DataTransferObjects.Token(userId, TokenStatus.Unauthorized);

                var accessToken = await _jwtTokenService.GetAccessTokenAsync(userId, expiresMilliseconds);

                var newToken = new Domain.DataTransferObjects.Token(Guid.NewGuid().ToString(), userId, accessToken,
                        TokenStatus.Authorized, expiresMilliseconds);

                await _tokenRepository.AddAsync(newToken.ToAuthorizedTokenEntity(ipAddress, expiresMilliseconds), cancellationToken);

                return newToken;
            }
            catch (Exception ex)
            {
                return new Domain.DataTransferObjects.Token(userId, TokenStatus.Error,
                    new TokenError("Unexpected error refresh async token", ex));
            }            
        }

        public async Task<bool> TryRevokeAsync(string id, string userId, string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                var token = await _tokenRepository.GetByIdAndUserIdAsync(id, userId);

                if (token != null)
                    return await _tokenRepository.UpdateAsync(token.ToRevokeTokenEntity(ipAddress), cancellationToken);

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

