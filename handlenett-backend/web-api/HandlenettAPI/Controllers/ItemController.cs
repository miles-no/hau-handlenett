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
        private readonly ILogger<ItemController> _logger;
        private readonly IConfiguration _config;
        private readonly CosmosDBService _cosmosDBService;

        public ItemController(ILogger<ItemController> logger, IConfiguration config)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _config = config ?? throw new ArgumentNullException(nameof(config));

                if (string.IsNullOrEmpty(_config.GetValue<string>("AzureCosmosDBSettings:DatabaseName"))
                    || string.IsNullOrEmpty(_config.GetValue<string>("AzureCosmosDBSettings:ContainerName"))
                    || string.IsNullOrEmpty(_config.GetValue<string>("AzureCosmosDBSettings:ConnectionString")))
                {
                    throw new ConfigurationErrorsException("Missing CosmosDB config values");
                }

                _cosmosDBService = new CosmosDBService(new CosmosClient(_config.GetValue<string>("AzureCosmosDBSettings:ConnectionString")), _config.GetValue<string>("AzureCosmosDBSettings:DatabaseName"), _config.GetValue<string>("AzureCosmosDBSettings:ContainerName"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed in constructor");
                throw new Exception("Something went wrong");
            }
        }

        [HttpGet(Name = "GetItems")]
        public async Task<ActionResult<List<Item>>> Get()
        {
            try
            {
                var sqlQuery = "SELECT * FROM c"; //ORDER BY c.created ? feiler dersom ingen items
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

        [HttpDelete("{id}", Name = "DeleteItem")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _cosmosDBService.Delete(id, GetUsername());
                //TODO: add feature if user tries to delete an item that someone else created (if GetUsername() != CreatedBy)
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
