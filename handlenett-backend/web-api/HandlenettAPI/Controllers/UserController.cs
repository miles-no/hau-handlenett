using Azure.Identity;
using HandlenettAPI.DTO;
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
    public class UserController : BaseController
    {

        public UserController(ILogger<UserController> logger, GraphServiceClient graphServiceClient, IConfiguration config, SlackService slackService)
            : base(logger, graphServiceClient, config, slackService)
        {
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<List<UserDTO>> Get()
        {
            try
            {
                await InitializeAsync();
                var dbService = new UserService(_config);
                return dbService.GetUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get users");
                throw new Exception("Failed to get users");
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<UserDTO> Get(Guid id)
        {
            try
            {
                await InitializeAsync();
                var dbService = new UserService(_config);
                return dbService.GetUser(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user");
                throw new Exception("Failed to get user");
            }
        }
    }
}
