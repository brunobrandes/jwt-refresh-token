using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Domain.Enums;
using Microsoft.Azure.Cosmos;

namespace Jwt.Refresh.Token.Infra.Cosmos.Entities.Repositories
{
	public class UserRepository : BaseRepository<Domain.Entities.User>, IUserRepository, IUserManagementRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public UserRepository(CosmosClient cosmosClient, string databaseId, string containerId) :
            base(cosmosClient, databaseId, containerId)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<Domain.Entities.User> AddAsync(Domain.Entities.User user, CancellationToken cancellationToken = default)
        {
            var itemResponse = await _container.CreateItemAsync(user, new PartitionKey(user.Id), cancellationToken: cancellationToken);
            user.Id = itemResponse.Resource.Id;
            return user;
        }

        public async Task<string> GetUserIdByIdAndPasswordAsync(string id, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await GetAsync(id, id);

                if (user.Status == UserStatus.Deactivated || user.Password != password)
                    return string.Empty;

                return user.Id;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return string.Empty;
            }     
        }

        public async Task<bool> UpdateAsync(Domain.Entities.User user, CancellationToken cancellationToken = default)
        {
            try
            {
                return await UpdateAsync(user, user.Id, user.Id, cancellationToken);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }            
        }
    }
}