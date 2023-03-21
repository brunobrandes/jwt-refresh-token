using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jwt.Refresh.Token.Domain.Services.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GetAccessTokenAsync(string userId, int expiresSeconds, CancellationToken cancellationToken = default(CancellationToken));
    }
}

