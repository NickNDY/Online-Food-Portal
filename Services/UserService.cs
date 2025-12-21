using MySqlConnector;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// User Service
    /// 
    /// Creates a user after Identity registers the user for matching the user to orders in the local MySQL database
    /// 
    /// </summary>
    /// <remarks>
    /// Constructs the MySQL connection string using the injected secrets repository
    /// </remarks>
    /// <param name="connectionStringBuilder">Injected Sql Connection string builder</param>
    public class UserService(ISqlConnectionStringBuilder connectionStringBuilder) : IUserService
    {
        private readonly string connectionString = connectionStringBuilder.GenerateConnectionString();

        /// <summary>
        /// Creates a user in the MySQL database
        /// </summary>
        /// <param name="username">The username of the created user</param>
        /// <returns>The number of rows affected. 1 for success, 0 for failure</returns>
        public int CreateUser(string username)
        {
            string sqlStatement = $"INSERT INTO users (username) VALUES (@username)";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, creating account");

                        MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                        command.Parameters.Add(new MySqlParameter("@username", MySqlDbType.VarChar)).Value = username;

                        affectedRows = command.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Created account: {(affectedRows == 1 ? "True" : "False")}");

                        connection.Close();
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                    }
                    catch (MySqlException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"MySQL Error: {ex.SqlState}: {ex.ErrorCode}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return affectedRows;
        }

        public int DeleteUserById(int id)
        {
            string sqlStatement = $"DELETE FROM users WHERE id = {id}";

            int affectedRows = 0;

            // Delete order item modifications
            // Delete order items
            // Delete orders
            // Delete user

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting account");

                        MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                        affectedRows = command.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Deleted account: {(affectedRows == 1 ? "True" : "False")}");

                        connection.Close();
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                    }
                    catch (MySqlException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"MySQL Error: {ex.SqlState}: {ex.ErrorCode}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return affectedRows;
        }


        /// <summary>
        /// Attempts to find a user by username
        /// </summary>
        /// <param name="username">The username to search for (case-insensitive)</param>
        /// <returns>The UserModel of the located user if found, otherwise null</returns>
        public UserModel? GetUserByUsername(string username)
        {
            string sqlStatement = $"SELECT * FROM users WHERE LOWER(username) = LOWER(@username)";

            UserModel? userModel = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, finding account by username");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@username", MySqlDbType.VarChar)).Value = username;

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        userModel = ParseReaderToUserModel(reader);

                        System.Diagnostics.Debug.WriteLine($"Found account with ID: {userModel.id} and UserName: {userModel.username}");
                    }
                    else
                        System.Diagnostics.Debug.WriteLine($"Failed to find account with UserName: {username}");
                    reader.Close();

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return userModel;
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <returns>A list of all users in the database</returns>
        public List<UserModel> GetUsers()
        {
            string sqlStatement = $"SELECT * FROM users";

            List<UserModel> userList = new List<UserModel>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, finding users");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        userList.Add(ParseReaderToUserModel(reader));
                    }
                    reader.Close();

                    System.Diagnostics.Debug.WriteLine($"Found users: {userList.Count}");

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return userList;
        }

        private UserModel ParseReaderToUserModel(MySqlDataReader reader)
        {
            return new UserModel(reader.GetInt32(0), reader.GetString(1));
        }
    }
}
