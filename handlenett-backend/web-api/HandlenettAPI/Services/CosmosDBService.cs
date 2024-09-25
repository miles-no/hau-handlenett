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

        public async Task<Item> Add (ItemPostDTO item)
        {
            var addItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Name = item.Name
            };

            var createdItem = await _container.CreateItemAsync<Item>(addItem, new PartitionKey(addItem.CreatedBy));
            return createdItem;
        }

        public async Task Delete (string id, string partition)
        {
            await _container.DeleteItemAsync<Item>(id, new PartitionKey(partition)); //les på partitions
        }

        public async Task<List<Item>> Get (string cosmosQuery)
        {
            var query = _container.GetItemQueryIterator<Item>(new QueryDefinition(cosmosQuery));
            List<Item> results = new List<Item> ();

            while (query.HasMoreResults) //best practice?
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results; //convert to ItemGetDTO
        }

        public async Task<Item> Update (ItemPutDTO item)
        {
            Item? updateItem = _container.GetItemLinqQueryable<Item>(true)
                .Where(x => x.Id == item.Id)
                .AsEnumerable()
                .FirstOrDefault();

            if (updateItem == null)
                throw new Exception("Wrong id");

            //updateItem.name = item.name;
            updateItem.UpdatedDate = DateTime.UtcNow;
            updateItem.UpdatedBy = "r";
            updateItem.IsCompleted = item.IsCompleted;

            var updatedItem = await _container.UpsertItemAsync<Item>(updateItem, new PartitionKey(updateItem.Id));
            return updatedItem;
        }
    }
}
