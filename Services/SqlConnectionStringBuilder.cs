using MySqlConnector;
using Online_Food_Portal.Interfaces;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// The SqlConnectionStringBuilder service used to generate the connection string once per service instance
    /// </summary>
    /// <param name="secretRepository">The secrets repository containing the SQL database password</param>
    public class SqlConnectionStringBuilder(ISecretRepository secretRepository) : ISqlConnectionStringBuilder
    {
        private readonly ISecretRepository secretRepository = secretRepository;

        /// <summary>
        /// Generates the connection string for SQL database access
        /// </summary>
        /// <returns>The string for connecting to the SQL database</returns>
        public string GenerateConnectionString()
        {
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

            return builder.ConnectionString;
        }
    }
}
