using Microsoft.Azure.Cosmos;
using MVC_Demo_Project.Models;

namespace MVC_Demo_Project.Data
{
    public class CosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync(Itemmodel item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public async Task<Itemmodel?> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Itemmodel> response = await _container.ReadItemAsync<Itemmodel>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Itemmodel>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Itemmodel>(new QueryDefinition(queryString));
            var results = new List<Itemmodel>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, Itemmodel item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<Itemmodel>(id, new PartitionKey(id));
        }
    }
}
