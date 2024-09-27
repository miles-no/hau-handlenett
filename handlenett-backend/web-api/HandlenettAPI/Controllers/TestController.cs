using HandlenettAPI.Models;
using HandlenettAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;
using System.Configuration;

namespace HandlenettAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IConfiguration _config;

        public TestController(ILogger<ItemController> logger, IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("Hello from Azure v2");
        }
    }
}
