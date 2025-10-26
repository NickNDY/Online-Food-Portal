using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Online_Food_Portal.Models;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// User Service
    /// 
    /// Creates a user after Identity registers the user for matching the user to orders in the local MySQL database
    /// 
    /// A lot of work went into testing why the SQL client was not connecting before realizing it was incompatible with MySQL
    /// Most of the methods were reset pending a successful connection before redeveloping
    /// </summary>
    public class UserService : IUserService
    {
        private readonly string connectionString;
        private readonly ISecretRepository secretRepository;

        /// <summary>
        /// Constructs the MySQL connection string using the injected secrets repository
        /// </summary>
        /// <param name="secretRepository">Injected secret repository</param>
        public UserService(ISecretRepository secretRepository)
        {
            this.secretRepository = secretRepository;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

            builder.Server = "localhost";
            builder.Port = 3306;
            builder.Database = "food_portal";
            builder.UserID = "root";
            builder.Password = secretRepository.GetSqlPassword();
            builder.SslMode = MySqlSslMode.Preferred;
            builder.ConnectionProtocol = MySqlConnectionProtocol.Tcp;
            builder.CancellationTimeout = 5;
            builder.ConnectionTimeout = 5;

            connectionString = builder.ConnectionString;

            System.Diagnostics.Debug.WriteLine($"Sql Connection String: {connectionString}");
        }

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
            throw new NotImplementedException();
        }

        public UserModel? GetUserById(int id)
        {
            throw new NotImplementedException();
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

        public List<UserModel> GetUsers()
        {
            throw new NotImplementedException();
        }

        public int UpdateUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        private UserModel ParseReaderToUserModel(MySqlDataReader reader)
        {
            return new UserModel(reader.GetInt32(0), reader.GetString(1));
        }
    }
}
