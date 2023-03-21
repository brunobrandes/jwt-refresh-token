using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jwt.Refresh.Token.Domain.Entities.Repositories
{
    public interface ITokenRepository
    {
        Task<Token> AddAsync(Entities.Token token,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Token> GetByIdAndUserIdAsync(string id, string userId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UpdateAsync(Domain.Entities.Token token,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}

