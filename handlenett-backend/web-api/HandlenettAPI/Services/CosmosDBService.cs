using Microsoft.Azure.Cosmos;
using HandlenettAPI.Models;
using System.Runtime.CompilerServices;
using HandlenettAPI.Interfaces;
using Microsoft.Graph;
using HandlenettAPI.DTO;

namespace HandlenettAPI.Services
{
    public class CosmosDBService//<T> where T : IBase //Lag generisk, støtte for andre enn Item struktur, basert på containerName?
    {
        private readonly Container _container;

        public CosmosDBService(CosmosClient client, string databaseName, string containerName)
        {
            _container = client.GetContainer(databaseName, containerName);
        }

        public async Task<Item> Add(ItemPostDTO item, string username)
        {
            var addItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Name = item.Name,
                CreatedBy = username,
                UpdatedBy = username
            };

            var createdItem = await _container.CreateItemAsync<Item>(addItem, new PartitionKey(addItem.CreatedBy));
            return createdItem;
        }

        public async Task Delete(string id, string partition)
        {
            await _container.DeleteItemAsync<Item>(id, new PartitionKey(partition)); //partitions er satt til createdBy
        }

        public async Task<List<Item>> GetByQuery(string cosmosQuery)
        {
            var query = _container.GetItemQueryIterator<Item>(new QueryDefinition(cosmosQuery));
            List<Item> results = new List<Item>();

            while (query.HasMoreResults) //best practice?
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results; //convert to ItemGetDTO
        }

        public Item? GetById(string id)
        {
            var item = _container.GetItemLinqQueryable<Item>(true)
                .Where(x => x.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            return item; //null, dto
        }

        public async Task<Item> Update(string id, ItemPutDTO item, string username)
        {
            Item? updateItem = _container.GetItemLinqQueryable<Item>(true)
                .Where(x => x.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (updateItem == null)
                throw new Exception("Wrong id");

            updateItem.Name = item.Name;
            updateItem.UpdatedDate = DateTime.UtcNow;
            updateItem.UpdatedBy = username;
            updateItem.IsCompleted = item.IsCompleted;

            var updatedItem = await _container.UpsertItemAsync<Item>(updateItem, new PartitionKey(updateItem.CreatedBy));
            return updatedItem;
        }
    }
}