using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HandlenettNotifications
{
    public class IsServiceUp
    {
        private readonly ILogger<IsServiceUp> _logger;

        public IsServiceUp(ILogger<IsServiceUp> logger)
        {
            _logger = logger;
        }

        [Function("IsServiceUp")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Yep, I'm alive!");
        }
    }
}
