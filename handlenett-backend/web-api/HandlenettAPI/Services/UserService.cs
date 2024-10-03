using Microsoft.Data.SqlClient;
using Microsoft.Graph;
using HandlenettAPI.Helpers;
using HandlenettAPI.DTO;
using Newtonsoft.Json.Linq;

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
            var myUser = await graphServiceClient.Me.Request().GetAsync();
            if (myUser == null)
            {
                throw new Exception("Could not get Ad profile");
            }
            var slackToken = _config.GetValue<string>("SlackBotUserOAuthToken");
            if (string.IsNullOrEmpty(slackToken)) throw new Exception("Missing config");


            //må reinstallere app i slack pga nytt scope users:read
            //var usersList = await slackService.GetUsersListAsync(slackToken);
            //linq filtrer på email --> get id

            //scope users:profile:read (denne er installert på miles workspace nå), hardkodet til roger slack id
            var slackUser = await slackService.GetUserProfileFromIdAsync(slackToken, "U06T83RHMFH");

            //AzureSQL
            if (!CheckIfUserExists(new Guid(myUser.Id)))
            {
                AddUser(myUser, slackUser.Id);
                await slackService.CopyImageToAzureBlobStorage(slackToken, slackUser);
            }
        }

        private bool CheckIfUserExists(Guid userId)
        {
            using (var db = new AzureSQLContext(_config))
            {
                var users = db.Users
                    .Where(u => u.Id == userId)
                    .FirstOrDefault();

                if (users != null)
                {
                    return true;
                }
                return false;
            }
        }

        private void AddUser(Microsoft.Graph.User user, string? slackUserId)
        {
            using (var db = new AzureSQLContext(_config))
            {
                var newUser = new Models.User
                {
                    Id = new Guid(user.Id),
                    FirstName = user.GivenName,
                    LastName = user.Surname,
                    SlackUserId = slackUserId
                };
                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }
    }
}
