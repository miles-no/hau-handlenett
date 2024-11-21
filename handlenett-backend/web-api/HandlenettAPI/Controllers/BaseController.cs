using HandlenettAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace HandlenettAPI.Controllers
{
    public abstract class BaseController : ControllerBase //not in use yet
    {
        protected readonly ILogger<BaseController> _logger;

        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
    }
}
