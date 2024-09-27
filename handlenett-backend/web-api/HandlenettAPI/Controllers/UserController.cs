using HandlenettAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace HandlenettAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IConfiguration _config;

        public UserController(ILogger<ItemController> logger, GraphServiceClient graphServiceClient, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        //[HttpGet(Name = "GetUser")]
        //public async Task<Microsoft.Graph.User> Get()
        //{
        //    return await _graphServiceClient.Me.Request().GetAsync(); ;
        //}

        //[HttpGet("Login")]
        //public async Task<bool> Login()
        //{
        //    return true;
        //}

        //[HttpGet("Logout")]
        //public async Task<bool> Logout()
        //{
        //    return true;
        //}
    }
}
