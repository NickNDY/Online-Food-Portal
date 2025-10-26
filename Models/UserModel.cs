namespace Online_Food_Portal.Models
{
    /// <summary>
    ///  User Model for handling data
    /// </summary>
    public class UserModel
    {
        public int id { get; set; }

        public string username { get; set; }

        public UserModel(int id, string username)
        {
            this.id = id;
            this.username = username;
        }
    }
}
