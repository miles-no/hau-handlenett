using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Graph;
using HandlenettAPI.Models;
using HandlenettAPI.Services;
using Microsoft.Azure.Cosmos;
using HandlenettAPI.DTO;

namespace HandlenettAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class ItemController : ControllerBase
    {
        private readonly GraphServiceClient _graphServiceClient;
        private static readonly List<Item> SampleItems = new List<Item>();
        private readonly ILogger<ItemController> _logger;
        private readonly IConfiguration _config;

        public readonly CosmosDBService _cosmosDBService; //trenger den å være public?

        public ItemController(ILogger<ItemController> logger, GraphServiceClient graphServiceClient, IConfiguration config)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
            //SampleItems.Add(new Item { Id = 1, Name = "Melk", CreatedBy = User.Identity.Name, UpdatedBy = User.Identity.Name });
            _config = config;
            var vClient = new CosmosClient(_config.GetValue<string>("AzureCosmosDBSettings:ConnectionString"));
            _cosmosDBService = new CosmosDBService(vClient, _config.GetValue<string>("AzureCosmosDBSettings:DatabaseName"), _config.GetValue<string>("AzureCosmosDBSettings:ContainerName"));
        }

        //[HttpGet(Name = "GetItems")]
        //public async Task<string> Get()
        //{
        //    //var user = await _graphServiceClient.Me.Request().GetAsync();
        //    return _config.GetValue<string>("AllowedHosts");
        //}

        [HttpGet(Name = "GetItems")]
        public async Task<IActionResult> Get() //best practice? Eller bare Task<returnType> ?
        {
            var sqlQuery = "SELECT * FROM c";
            var result = await _cosmosDBService.Get(sqlQuery);
            return Ok(result); //hva med sortering? i SQL Query eller i.net?
        }

        [HttpPost(Name = "AddItem")]
        public async Task<IActionResult> Add(ItemPostDTO item)
        {
            var result = await _cosmosDBService.Add(item);
            return Ok(result);
        }

        [HttpPut(Name = "UpdateItem")]
        public async Task<IActionResult> Update(ItemPutDTO item)
        {
            var result = await _cosmosDBService.Update(item);
            return Ok(result);
        }

        [HttpDelete(Name = "DeleteItem")]
        public async Task<IActionResult> Delete(string id)
        {
            await _cosmosDBService.Delete(id, id); //påkrevd partition, konfigurert til å alltid være lik id i azure cosmosdb
            return Ok();
        }
    }
}
