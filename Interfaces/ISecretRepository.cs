namespace Online_Food_Portal.Interfaces
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
            string path = Path.Combine(new string[] { Directory.GetCurrentDirectory(), "sqlpassword.txt" });
            if (File.Exists(path))
            {
                string sqlPassword = File.ReadAllText(path).Trim();

                if (sqlPassword.Length > 0) return sqlPassword;
            }

            IConfigurationSection section = configuration.GetSection("SqlDatabaseConnectionPassword");
            if (section != null && section.Exists() && section.Value != null)
                return section.Value;

            System.Diagnostics.Debug.WriteLine("Configuration Section 'SqlDatabaseConnectionPassword' not found");

            return string.Empty;
        }
    }
}
