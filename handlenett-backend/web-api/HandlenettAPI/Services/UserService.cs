using Microsoft.Data.SqlClient;
using Microsoft.Graph;
using HandlenettAPI.Helpers;
using HandlenettAPI.DTO;
using HandlenettAPI.Models;

namespace HandlenettAPI.Services
{
    public class UserService
    {
        private readonly AzureSQLContext _dbContext;
        private readonly AzureBlobStorageService _blobStorageService;
        private readonly string _containerName;

        public UserService(AzureSQLContext dbContext, AzureBlobStorageService blobStorageService, string containerName)
        {
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
            _containerName = containerName;
        }

        public List<UserDTO> GetUsers()
        {
            try
            {
                var users = _dbContext.Users
                    .Where(u => u.IsDeleted == false)
                    .OrderBy(u => u.Name)
                    .ToList();

                if (users == null || users.Count == 0) return new List<UserDTO>();

                var usersDTO = users
                    .Select(user => ConvertUserToUserDTO(user))
                    .ToList();

                return usersDTO;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private UserDTO ConvertUserToUserDTO(Models.User user)
        {
            var userDTO = user.ConvertTo<UserDTO>();
            userDTO.ImageUrl = $"{user.ImageUrl}?{_blobStorageService.GenerateContainerSasToken(_containerName)}";
            return userDTO;
        }

        public UserDTO GetUser(Guid id)
        {
            try
            {
                var user = _dbContext.Users
                    .Where(u => u.Id == id)
                    .FirstOrDefault();

                if (user == null) throw new InvalidOperationException("User not found");

                return ConvertUserToUserDTO(user);
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
            var SQLUser = GetUserWithDetails(new Guid(ADUser.Id));

            //AzureSQL and slack
            if (SQLUser == null)
            {
                var slackUser = await slackService.GetUserByEmailAsync(ADUser.Mail);
                if (slackUser != null)
                {
                    slackUser.ImageUrlBlobStorage = await slackService.CopyImageToAzureBlobStorage(slackUser);
                }
                AddUser(ADUser, slackUser);
            }
        }

        private Models.User? GetUserWithDetails(Guid userId)
        {
            var user = _dbContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            return user;
        }

        private void AddUser(Microsoft.Graph.User user, SlackUser? slackUser)
        {
            var newUser = new Models.User
            {
                Id = new Guid(user.Id),
                FirstName = user.GivenName,
                LastName = user.Surname,
                SlackUserId = slackUser?.Id,
                ImageUrl = slackUser?.ImageUrlBlobStorage,
            };
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }
    }
}
