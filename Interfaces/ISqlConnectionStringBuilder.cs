using Microsoft.AspNetCore.Components;
using MySqlConnector;

namespace Online_Food_Portal.Interfaces
{
    public interface ISqlConnectionStringBuilder
    {
        public string GenerateConnectionString();
    }
}
