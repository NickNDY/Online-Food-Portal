using MySqlConnector;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;
using System.Collections;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Store settings data service
    /// </summary>
    /// <param name="connectionStringBuilder">The SqlConnectionStringBuilder service used to generate the connection string once</param>
    public class StoreSettingsService(ISqlConnectionStringBuilder connectionStringBuilder) : IStoreSettingsService
    {
        private readonly string connectionString = connectionStringBuilder.GenerateConnectionString();

        /// <summary>
        /// Returns the store settings
        /// </summary>
        /// <returns>The store settings. Returns null if the settings have not been set (they are by default)</returns>
        public StoreSettingsModel GetStoreSettings()
        {
            string sqlStatement = $"SELECT * FROM store_settings LIMIT 1";

            StoreSettingsModel? storeSettings = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving store settings");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        storeSettings = ParseReaderToStoreSettingsModel(reader);

                        System.Diagnostics.Debug.WriteLine("Retrieved store settings");
                    }
                    reader.Close();

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            if (storeSettings == null)
            {
                StoreSettingsModel defaultSettings = new StoreSettingsModel(TimeSpan.FromHours(8), TimeSpan.FromHours(20), new bool[] { true, true, true, true, true, true, true }, true, "Not Set", "Not Set");
                SetStoreSettings(defaultSettings);

                return defaultSettings;
            }

            return storeSettings;
        }

        /// <summary>
        /// Sets the store settings
        /// </summary>
        /// <param name="model">The model containing the new store settings</param>
        /// <returns>The number of affected rows. 1 = success, 0 = failure</returns>
        public int SetStoreSettings(StoreSettingsModel model)
        {
            string sqlStatement = $"UPDATE store_settings SET open_time = @open_time, close_time = @close_time, business_days = @business_days, ordering_enabled = @ordering_enabled, store_address = @store_address, store_phone = @store_phone WHERE id = 1";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, updating store settings");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add("@open_time", System.Data.DbType.Time).Value = model.open_time;
                    command.Parameters.Add("@close_time", System.Data.DbType.Time).Value = model.close_time;
                    string businessDayString = "";
                    foreach (bool day in model.business_days)
                    {
                        businessDayString += day ? "1" : "0";
                    }
                    command.Parameters.Add("@business_days", System.Data.DbType.String).Value = businessDayString;
                    command.Parameters.Add("@ordering_enabled", System.Data.DbType.Boolean).Value = model.ordering_enabled;
                    command.Parameters.Add("@store_address", System.Data.DbType.String).Value = model.store_address;
                    command.Parameters.Add("@store_phone", System.Data.DbType.String).Value = model.store_phone;

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Updated Settings: {(affectedRows == 1 ? "True" : "False")}");

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return affectedRows;
        }

        private static StoreSettingsModel ParseReaderToStoreSettingsModel(MySqlDataReader reader)
        {
            string businessDays = reader.GetString(3);
            bool[] boolArray = new bool[7];

            for (int i = 0; i < businessDays.Length && i < boolArray.Length; i++)
                boolArray[i] = businessDays[i] == '1';

            return new StoreSettingsModel(
                reader.GetTimeSpan(1),
                reader.GetTimeSpan(2),
                boolArray,
                reader.GetBoolean(4),
                reader.GetString(5),
                reader.GetString(6));
        }
    }
}
