using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Graph;
using HandlenettAPI.Models;
using HandlenettAPI.Services;
using Microsoft.Azure.Cosmos;
using HandlenettAPI.DTO;
using System.Configuration;

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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(logger));

            _graphServiceClient = graphServiceClient; //se docs for muligheter her

            if (string.IsNullOrEmpty(_config.GetValue<string>("AzureCosmosDBSettings:DatabaseName"))
                || string.IsNullOrEmpty(_config.GetValue<string>("AzureCosmosDBSettings:ContainerName")))
            {
                throw new ConfigurationErrorsException("Missing values");
            }

            var vClient = new CosmosClient(_config.GetValue<string>("AzureCosmosDBSettings:ConnectionString"));
            _cosmosDBService = new CosmosDBService(vClient, _config.GetValue<string>("AzureCosmosDBSettings:DatabaseName"), _config.GetValue<string>("AzureCosmosDBSettings:ContainerName"));
        }

        //[HttpGet(Name = "testing graph")]
        //public async Task<string> Get()
        //{
        //    var user = await _graphServiceClient.Me.Request().GetAsync();
        //    return _config.GetValue<string>("AllowedHosts");
        //}

        [HttpGet(Name = "GetItems")]
        public async Task<ActionResult<List<Item>>> Get() //best practice? Eller bare Task<returnType> ?
        {
            try
            {
            var sqlQuery = "SELECT * FROM c"; //ORDER BY c.created ?
            var result = await _cosmosDBService.GetByQuery(sqlQuery);
            return Ok(result); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get items");
                return StatusCode(500, new { message = "Failed to get items" });
            }
        }

        [HttpGet("{id}", Name = "GetItem")]
        public ActionResult<Item> Get(string id)
        {
            try
            {
                var result = _cosmosDBService.GetById(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Item with ID {id} not found." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get item");
                return StatusCode(500, new { message = "Failed to get item" });
            }
        }

        [HttpPost(Name = "AddItem")]
        public async Task<IActionResult> Add(ItemPostDTO item)
        {
            try
            { 
            var result = await _cosmosDBService.Add(item, GetUsername());
            return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add item");
                return StatusCode(500, new { message = "Failed to add item" });
            }
        }

        [HttpPut("{id}", Name = "UpdateItem")]
        public async Task<IActionResult> Update(string id, [FromBody] ItemPutDTO item)
        {
            try
            {
                var result = await _cosmosDBService.Update(id, item, GetUsername());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update item");
                return StatusCode(500, new { message = "Failed to update item" });
            }
        }

        [HttpDelete(Name = "DeleteItem")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _cosmosDBService.Delete(id, GetUsername());
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete item");
                return StatusCode(500, new { message = "Failed to delete item" });
            }
        }

        private string GetUsername()
        {
            if (HttpContext?.User?.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                throw new UnauthorizedAccessException("User is not authenticated or username is missing.");
            }

            return HttpContext.User.Identity.Name;
        }
    }
}
