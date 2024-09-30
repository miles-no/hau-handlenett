using HandlenettAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Graph;

namespace HandlenettAPI.Services
{

    public class UserService
    {
        private IConfiguration _config;
        public UserService(IConfiguration config) 
        {
            _config = config;
        }


        public List<Models.User> GetUsers()
        {
            try
            {
                using (var db = new AzureSQLContext(_config))
                {
                    var users = db.Users
                        .Where(u => u.IsDeleted == false)
                        .OrderBy(u => u.Name)
                        .ToList();

                    return users;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.User> AddUserIfNotExists(GraphServiceClient graphServiceClient)
        {
            await graphServiceClient.Me.Request().GetAsync();

            return new Models.User();
        }
    }
}
