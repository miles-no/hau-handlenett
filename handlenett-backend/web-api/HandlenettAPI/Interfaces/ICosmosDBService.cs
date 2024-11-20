using HandlenettAPI.DTO;
using HandlenettAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HandlenettAPI.Interfaces
{
    public interface ICosmosDBService
    {
        Task<List<Item>> GetByQuery(string cosmosQuery);
        Task<Item> Add(ItemPostDTO item, string username);
        Task Delete(string id, string partition);
        Item? GetById(string id);
        Task<Item> Update(string id, ItemPutDTO item, string username);
    }
}
