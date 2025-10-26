using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    public interface IUserService
    {
        // Create
        public int CreateUser(string username);

        // Read
        public List<UserModel> GetUsers();
        public UserModel? GetUserById(int id);
        public UserModel? GetUserByUsername(string username);

        // Update
        public int UpdateUser(UserModel user);

        // Delete
        public int DeleteUserById(int id);
    }
}
