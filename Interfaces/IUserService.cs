using Online_Food_Portal.Models;

namespace Online_Food_Portal.Interfaces
{
    public interface IUserService
    {
        // Create
        public int CreateUser(string username);

        // Read
        public List<UserModel> GetUsers();
        public UserModel? GetUserByUsername(string username);

        // Delete
        public int DeleteUserById(int id);
    }
}
