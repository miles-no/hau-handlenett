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
            var myUser = await graphServiceClient.Me.Request().GetAsync();

            if (myUser == null)
            {
                throw new Exception("Could not get Ad profile");
            }

            if (!CheckIfUserExists(new Guid(myUser.Id)))
            {
                AddUser(myUser);
            }

            return new Models.User();
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

        private bool AddUser(Microsoft.Graph.User user)
        {
            using (var db = new AzureSQLContext(_config))
            {
                var newUser = new Models.User 
                { 
                    Id = new Guid(user.Id),
                    FirstName = user.GivenName,
                    LastName = user.Surname
                };
                db.Users.Add(newUser);
                db.SaveChanges();
                return true;
            }
        }
    }
}
