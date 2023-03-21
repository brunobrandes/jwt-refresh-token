using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Jwt.Refresh.Token.Domain.Services.Interfaces
{
    public interface IClaimsIdentityService
    {
        Task<ClaimsIdentity> GetAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
    }
}

