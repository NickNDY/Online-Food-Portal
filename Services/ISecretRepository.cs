namespace Online_Food_Portal.Services
{
    public interface ISecretRepository
    {
        string GetSqlPassword();
    }

    public class SecretRepository : ISecretRepository
    {
        public IConfiguration configuration;

        public SecretRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetSqlPassword()
        {
            IConfigurationSection section = configuration.GetSection("SqlDatabaseConnectionPassword");
            if (section != null && section.Exists() && section.Value != null)
                return section.Value;

            System.Diagnostics.Debug.WriteLine("Configuration Section 'SqlDatabaseConnectionPassword' not found");

            return string.Empty;
        }
    }
}
