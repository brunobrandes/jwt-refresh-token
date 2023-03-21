using System;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Refresh.Token.Domain.Entities;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Net;

namespace Jwt.Refresh.Token.Infra.Cosmos.Entities.Repositories
{
    public class TokenRepository : BaseRepository<Domain.Entities.Token>, ITokenRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public TokenRepository(CosmosClient cosmosClient, string databaseId, string containerId) :
            base(cosmosClient, databaseId, containerId)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<Domain.Entities.Token> AddAsync(Domain.Entities.Token token, CancellationToken cancellationToken = default)
        {
                var itemResponse = await _container.CreateItemAsync(token, new PartitionKey(token.UserId), cancellationToken: cancellationToken);
                return token;            
        }

        public async Task<Domain.Entities.Token> GetByIdAndUserIdAsync(string id, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.GetAsync(id, userId, cancellationToken: cancellationToken);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }            
        }

        public async Task<bool> UpdateAsync(Domain.Entities.Token token, CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.UpdateAsync(token, token.Id, token.UserId, cancellationToken);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }            
        }
    }
}

