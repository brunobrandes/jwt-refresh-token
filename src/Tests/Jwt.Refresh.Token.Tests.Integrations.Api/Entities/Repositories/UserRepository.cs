using Jwt.Refresh.Token.Domain.Entities.Repositories;

namespace Jwt.Refresh.Token.Tests.Integrations.Api.Entities.Repositories;

public class UserRepository : IUserRepository
{
    public Task<string> GetUserIdByIdAndPasswordAsync(string id, string password, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string>("user_test@gmail.com");
    }
}