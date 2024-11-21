using Microsoft.Graph;

namespace HandlenettAPI.Services
{
    public class UserInitializationService
    {
        private readonly UserService _userService;
        private readonly GraphServiceClient _graphClient;
        private readonly SlackService _slackService;

        public UserInitializationService(UserService userService, GraphServiceClient graphClient, SlackService slackService)
        {
            _userService = userService;
            _graphClient = graphClient;
            _slackService = slackService;
        }

        public async Task EnsureUserExistsAsync()
        {
            await _userService.AddUserIfNotExists(_graphClient, _slackService);
        }
    }
}
