using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jwt.Refresh.Token.Application.Interfaces
{
	public interface ITokenAppService
	{
        Task<Domain.DataTransferObjects.Token> CreateAsync(string userId, string password, int expiresMilliseconds, string ipAddress,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Domain.DataTransferObjects.Token> RefreshAsync(string tokenIdß, string userId, int expiresMilliseconds, string ipAddress,
             CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> TryRevokeAsync(string tokenId, string userId, string ipAddress,
       CancellationToken cancellationToken = default(CancellationToken));
    }
}