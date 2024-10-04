using Microsoft.Data.SqlClient;
using Microsoft.Graph;
using HandlenettAPI.Helpers;
using HandlenettAPI.DTO;
using Newtonsoft.Json.Linq;
using HandlenettAPI.Models;

namespace HandlenettAPI.Services
{

    public class UserService
    {
        private IConfiguration _config;
        public UserService(IConfiguration config)
        {
            _config = config;
        }


        public List<UserDTO> GetUsers()
        {
            try
            {
                using (var db = new AzureSQLContext(_config))
                {
                    var users = db.Users
                        .Where(u => u.IsDeleted == false)
                        .OrderBy(u => u.Name)
                        .ToList();

                    return users == null ? [] : users.ConvertToList<UserDTO>();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public UserDTO GetUser(Guid id)
        {
            try
            {
                using (var db = new AzureSQLContext(_config))
                {
                    var user = db.Users
                        .Where(u => u.Id == id)
                        .FirstOrDefault();

                    return user == null ? throw new InvalidOperationException("User not found") : user.ConvertTo<UserDTO>();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task AddUserIfNotExists(GraphServiceClient graphServiceClient, SlackService slackService)
        {
            //Ad
            var ADUser = await graphServiceClient.Me.Request().GetAsync();
            if (ADUser == null)
            {
                throw new Exception("Could not get Ad profile");
            }
            var slackToken = _config.GetValue<string>("SlackBotUserOAuthToken");
            if (string.IsNullOrEmpty(slackToken)) throw new Exception("Missing config");

            var SQLUser = GetMyUser(new Guid(ADUser.Id));

            //AzureSQL og slack
            if (SQLUser == null)
            {
                //Denne funker bare når admin har godkjent nytt scope for slack app
                var slackUser = await slackService.GetUserIdByEmailAsync(slackToken, ADUser.Mail);
                if (slackUser != null)
                {
                    //TODO: denne må returnere url SAS token. Når det passes til frontend må det genereres SAS token som sendes med url
                    await slackService.CopyImageToAzureBlobStorage(slackToken, slackUser);
                }
                AddUser(ADUser, slackUser);
            }
        }

        private Models.User? GetMyUser(Guid userId)
        {
            using (var db = new AzureSQLContext(_config))
            {
                var user = db.Users
                    .Where(u => u.Id == userId)
                    .FirstOrDefault();

                return user;
            }
        }

        private void AddUser(Microsoft.Graph.User user, SlackUser? slackUser)
        {
            using (var db = new AzureSQLContext(_config))
            {
                var newUser = new Models.User
                {
                    Id = new Guid(user.Id),
                    FirstName = user.GivenName,
                    LastName = user.Surname,
                    SlackUserId = slackUser?.Id,
                    ImageUrl = slackUser?.ImageUrl
                };
                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }
    }
}
