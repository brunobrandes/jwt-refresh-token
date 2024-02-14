using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Refresh.Token.Domain.DataTransferObjects;
using Jwt.Refresh.Token.Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace Jwt.Refresh.Token.Infra.Cosmos.Entities.Repositories
{
	public interface IBaseRepository<TEntity>
		where TEntity : class
	{
        Task<TEntity> GetAsync(string id, string partitionKey, ItemRequestOptions itemRequestOptions = default,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> GetAllAsync(string partitionKey, QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default);      

        Task<bool> UpdateAsync(TEntity entity, string id, string partitionKey,
            CancellationToken cancellationToken = default);
    }

    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public BaseRepository(CosmosClient cosmosClient, string databaseId, string containerId)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<TEntity> GetAsync(string id, string partitionKey, ItemRequestOptions itemRequestOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await _container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey), itemRequestOptions);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string partitionKey, QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default)
        {
            var queryResultSetIterator = _container.GetItemQueryIterator<TEntity>(queryDefinition, requestOptions: new QueryRequestOptions
            {
                PartitionKey =  new PartitionKey(partitionKey),
            });

            List<TEntity> allItems = new List<TEntity>();

            while (queryResultSetIterator.HasMoreResults)
            {
                var contentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach (TEntity entity in contentResultSet)
                {
                    allItems.Add(entity);
                }
            }

            return allItems;
        }

        public async Task<bool> UpdateAsync(TEntity entity, string id, string partitionKey, CancellationToken cancellationToken = default)
        {
            var itemResponse = await _container.ReplaceItemAsync<TEntity>(entity, id, new PartitionKey(partitionKey));
            return itemResponse?.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}

