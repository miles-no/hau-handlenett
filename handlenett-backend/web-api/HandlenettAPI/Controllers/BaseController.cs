using HandlenettAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace HandlenettAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly IConfiguration _config;
        protected readonly GraphServiceClient _graphClient;
        protected readonly SlackService _slackService;

        protected BaseController(ILogger<BaseController> logger, GraphServiceClient graphServiceClient, IConfiguration config, SlackService slackService)
        {
            _logger = logger;
            _config = config;
            _graphClient = graphServiceClient;
            _slackService = slackService;
        }

        protected async Task InitializeAsync()
        {
            try
            {
                var dbService = new UserService(_config);
                await dbService.AddUserIfNotExists(_graphClient, _slackService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Not valid user");
                throw new Exception("Not valid user");
            }
        }
    }
}
