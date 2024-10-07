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
                var SASToken = GetAzureBlobStorageUserImageSASToken();

                using (var db = new AzureSQLContext(_config))
                {
                    var users = db.Users
                        .Where(u => u.IsDeleted == false)
                        .OrderBy(u => u.Name)
                        .ToList();
                    if (users == null) return [];

                    var usersDTO = new List<UserDTO>();

                    foreach (var user in users)
                    {
                        var userDTO = ConvertUserToUserDTO(user, SASToken);
                        usersDTO.Add(userDTO);
                    }

                    return usersDTO;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private string GetAzureBlobStorageUserImageSASToken()
        {
            var containerName = _config.GetValue<string>("AzureStorage:ContainerNameUserImages");
            if (string.IsNullOrEmpty(containerName)) throw new Exception("Missing config");
            var azureService = new AzureBlobStorageService(containerName, _config);
            return azureService.GenerateContainerSasToken();
        }

        private UserDTO ConvertUserToUserDTO(Models.User user, string SASToken)
        {
            var userDTO = user.ConvertTo<UserDTO>();
            userDTO.ImageUrl = $"{user.ImageUrl}?{SASToken}";
            return userDTO;
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

                    if (user == null) throw new InvalidOperationException("User not found");

                    var SASToken = GetAzureBlobStorageUserImageSASToken();
                    return ConvertUserToUserDTO(user, SASToken);
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

            var SQLUser = GetUserWithDetails(new Guid(ADUser.Id));

            //AzureSQL og slack
            if (SQLUser == null)
            {
                var slackUser = await slackService.GetUserByEmailAsync(slackToken, ADUser.Mail);
                if (slackUser != null)
                {
                    slackUser.ImageUrlBlobStorage = await slackService.CopyImageToAzureBlobStorage(slackToken, slackUser);
                }
                AddUser(ADUser, slackUser);
            }
        }

        private Models.User? GetUserWithDetails(Guid userId)
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
                    ImageUrl = slackUser?.ImageUrlBlobStorage,
                };
                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }
    }
}
