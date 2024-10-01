using Azure.Identity;
using HandlenettAPI.Models;
using HandlenettAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace HandlenettAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IConfiguration _config;
        private readonly GraphServiceClient _graphClient;

        public UserController(ILogger<ItemController> logger, GraphServiceClient graphServiceClient, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _graphClient = graphServiceClient;
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<List<Models.User>> Get()
        {
            try
            {
                var dbService = new UserService(_config);
                await dbService.AddUserIfNotExists(_graphClient);

                return dbService.GetUsers(); //endre til dto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get users");
                throw new Exception("Failed to get users");
            }
        }
    }
}
