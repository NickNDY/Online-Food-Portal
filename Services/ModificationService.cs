using MySqlConnector;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Item Modification data service
    /// </summary>
    /// <param name="sqlConnectionStringBuilder">The SqlConnectionStringBuilder service used to generate the connection string once</param>
    public class ModificationService(ISqlConnectionStringBuilder sqlConnectionStringBuilder) : IModificationService
    {
        private readonly string connectionString = sqlConnectionStringBuilder.GenerateConnectionString(); // SQL connection string

        /// <summary>
        /// Create item modification
        /// </summary>
        /// <param name="name">Name of the modification</param>
        /// <param name="description">Description of the modification</param>
        /// <param name="price_offset">Price offset of the modification, affects item price</param>
        /// <param name="stock">Stock of the item modification, -1 for unlimited</param>
        /// <param name="defaultModification">Whether the modification is defaultModification on an item, false for optional</param>
        /// <param name="hidden">Whether the modification is hidden (disabled)</param>
        /// <param name="items_id">ID of the item this modification applies to</param>
        /// <returns>The ID of the created modification, or -1 if creation failed</returns>
        public int CreateModification(string name, string description, decimal price_offset, int stock, bool defaultModification, bool hidden, int items_id)
        {
            string sqlStatement =
                $"INSERT INTO modifications (name, description, price_offset, stock, defaultModification, hidden, items_id) " +
                $"VALUES (@name, @description, @price_offset, @stock, @defaultModification, @hidden, @items_id)";

            int createdModificationId = -1;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, creating modification");

                        MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                        command.Parameters.Add(new MySqlParameter("@name", MySqlDbType.VarChar)).Value = name;
                        command.Parameters.Add(new MySqlParameter("@description", MySqlDbType.VarChar)).Value = description;
                        command.Parameters.Add(new MySqlParameter("@price_offset", MySqlDbType.Decimal)).Value = price_offset;
                        command.Parameters.Add(new MySqlParameter("@stock", MySqlDbType.Int32)).Value = stock;
                        command.Parameters.Add(new MySqlParameter("@defaultModification", MySqlDbType.Bool)).Value = defaultModification;
                        command.Parameters.Add(new MySqlParameter("@hidden", MySqlDbType.Bool)).Value = hidden;
                        command.Parameters.Add(new MySqlParameter("@items_id", MySqlDbType.Int32)).Value = items_id;

                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows != -1)
                        {
                            sqlStatement = "SELECT LAST(id) FROM modifications";

                            command = new MySqlCommand(sqlStatement, connection);

                            object? lastModificationId = command.ExecuteScalar();

                            if (lastModificationId != null)
                                createdModificationId = (int)lastModificationId;
                        }

                        System.Diagnostics.Debug.WriteLine($"Created modification: {(affectedRows == 1 ? "True" : "False")}");

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

            return createdModificationId;
        }

        /// <summary>
        /// Gets all modifications
        /// </summary>
        /// <returns>A list of all modifications</returns>
        public List<ModificationModel> GetModifications()
        {
            string sqlStatement = "SELECT * FROM modifications";

            List<ModificationModel> modificationModels = new List<ModificationModel>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, selecting all modifications");

                        MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            modificationModels.Add(ParseReaderToModificationModel(reader));
                        }

                        System.Diagnostics.Debug.WriteLine($"Selected modification{(modificationModels.Count != 1 ? "s" : "")}: {modificationModels.Count}");

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

            return modificationModels;
        }

        /// <summary>
        /// Gets all modifications for a specific item
        /// </summary>
        /// <param name="items_id"></param>
        /// <returns></returns>
        public List<ModificationModel> GetModificationsByItemId(int items_id)
        {
            string sqlStatement = "SELECT * FROM modifications WHERE items_id = @items_id";

            List<ModificationModel> modificationModels = new List<ModificationModel>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Connection to MySQL successful, selecting modifications with items_id: {items_id}");

                        MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                        command.Parameters.Add(new MySqlParameter("@items_id", MySqlDbType.Int32)).Value = items_id;

                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            modificationModels.Add(ParseReaderToModificationModel(reader));
                        }

                        System.Diagnostics.Debug.WriteLine($"Selected modification{(modificationModels.Count != 1 ? "s" : "")}: {modificationModels.Count}");

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

            return modificationModels;
        }

        /// <summary>
        /// Gets a specific modification
        /// </summary>
        /// <param name="id">The ID of the modification</param>
        /// <returns>The requested modification, or null if not found</returns>
        public ModificationModel? GetModification(int id)
        {
            string sqlStatement = "SELECT * FROM modifications WHERE id = @id";

            ModificationModel? modificationModel = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving modification");

                        MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                        command.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)).Value = id;

                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                            modificationModel = ParseReaderToModificationModel(reader);

                        System.Diagnostics.Debug.WriteLine($"Found modification: {modificationModel != null}");

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

            return modificationModel;
        }

        /// <summary>
        /// Updates a modification
        /// </summary>
        /// <param name="model">The modification to update</param>
        /// <returns>The number of rows affected. 1 = successful, 0 = failed</returns>
        public int UpdateModification(ModificationModel modification)
        {
            string sqlStatement = $"UPDATE modifications SET name = @name, description = @description, price_offset = @price_offset, stock = @stock, defaultModification = @defaultModification, hidden = @hidden WHERE id = @id";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, updating modification");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@name", MySqlDbType.VarChar)).Value = modification.name;
                    command.Parameters.Add(new MySqlParameter("@description", MySqlDbType.VarChar)).Value = modification.description;
                    command.Parameters.Add(new MySqlParameter("@price", MySqlDbType.Decimal)).Value = modification.price_offset;
                    command.Parameters.Add(new MySqlParameter("@stock", MySqlDbType.Int32)).Value = modification.stock;
                    command.Parameters.Add(new MySqlParameter("@defaultModification", MySqlDbType.Bool)).Value = modification.defaultModification;
                    command.Parameters.Add(new MySqlParameter("@hidden", MySqlDbType.Bool)).Value = modification.hidden;
                    command.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)).Value = modification.id;

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Updated modification: {(affectedRows == 1 ? "True" : "False")}");

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

        /// <summary>
        /// Deletes a modification
        /// NOTE: For testing purposes only! Will fail if used on a production modification in an order
        /// </summary>
        /// <param name="id">The ID of the modification to delete</param>
        /// <returns>The number of affected rows. 1 = success, 0 = failure</returns>
        public int DeleteModification(int id)
        {
            int affectedRows = 0;

            string sqlStatement = $"DELETE FROM modifications WHERE id = @id";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting modification");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)).Value = id;

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted modification: {(affectedRows == 1 ? "True" : "False")}");

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

        /// <summary>
        /// Deletes modifications for a specific item
        /// NOTE: For testing purposes only! Will fail if used on a production item in an order
        /// </summary>
        /// <param name="items_id">The ID of the item to delete the modifications of</param>
        /// <returns>The number of affected rows. 1+ = success, 0 = failure</returns>
        public int DeleteModificationsByItemId(int items_id)
        {
            int affectedRows = 0;

            string sqlStatement = $"DELETE FROM modifications WHERE items_id = @items_id";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting modifications");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@items_id", MySqlDbType.Int32)).Value = items_id;

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted modification: {(affectedRows > 0 ? "True" : "False")} ({affectedRows})");

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

        /// <summary>
        /// Adds a modification to an item in an order
        /// </summary>
        /// <param name="order_items_id">The ID of the order item to add the modification to</param>
        /// <param name="modifications_id">The ID of the modification to add to the item</param>
        /// <returns>The number of affected rows. 1 = success, 0 = failure</returns>
        public int AddOrderModification(int order_items_id, int modifications_id)
        {
            string sqlStatement =
                $"INSERT INTO order_modifications (order_items_id, modifications_id) " +
                $"VALUES (@order_items_id, @modifications_id)";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, creating modification");

                        MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                        command.Parameters.Add(new MySqlParameter("@order_items_id", MySqlDbType.Int32)).Value = order_items_id;
                        command.Parameters.Add(new MySqlParameter("@modifications_id", MySqlDbType.Int32)).Value = modifications_id;

                        affectedRows = command.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Created order item modification: {(affectedRows == 1 ? "True" : "False")}");

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
        /// Delete a modification from an order by ID
        /// </summary>
        /// <param name="id">The ID of the order modification</param>
        /// <returns>The number of affected rows. 1 for success, 0 for failure.</returns>
        public int DeleteOrderModification(int id)
        {
            int affectedRows = 0;

            string sqlStatement = $"DELETE FROM order_modifications WHERE id = {id}";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting modification");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted modification: {(affectedRows == 1 ? "True" : "False")} ({affectedRows})");

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

        /// <summary>
        /// Delete all modifications from an order item by order item ID
        /// </summary>
        /// <param name="order_items_id">The ID of the order item</param>
        /// <returns>The number of rows affected. x > 0 for success, 0 for failure.</returns>
        public int DeleteAllOrderModificationsByOrderItemId(int order_items_id)
        {
            int affectedRows = 0;

            string sqlStatement = $"DELETE FROM order_modifications WHERE order_items_id = {order_items_id}";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting modifications");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted modification: {(affectedRows > 0 ? "True" : "False")} ({affectedRows})");

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

        private static ModificationModel ParseReaderToModificationModel(MySqlDataReader reader)
        {
            return new ModificationModel(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDecimal(3),
                reader.GetInt32(4),
                reader.GetBoolean(5),
                reader.GetBoolean(6),
                reader.GetInt32(7));
        }
    }
}
