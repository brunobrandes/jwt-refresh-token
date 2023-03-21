using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jwt.Refresh.Token.Domain.Entities.Repositories
{
    public interface IUserManagementRepository
    {
        Task<Domain.Entities.User> AddAsync(Domain.Entities.User user,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UpdateAsync(Domain.Entities.User user,
            CancellationToken cancellationToken = default);
    }
}

