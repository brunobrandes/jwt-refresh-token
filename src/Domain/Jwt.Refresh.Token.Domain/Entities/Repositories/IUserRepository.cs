using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jwt.Refresh.Token.Domain.Entities.Repositories
{
    public interface IUserRepository
    {
        Task<string> GetUserIdByIdAndPasswordAsync(string id, string password, CancellationToken cancellationToken = default(CancellationToken));
    }
}

