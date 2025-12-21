using MySqlConnector;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Item data service
    /// 
    /// Handles item-specific SQL transactions
    /// </summary>
    /// <remarks>
    /// Constructs the item service with injected services
    /// </remarks>
    /// <param name="connectionStringBuilder">The SqlConnectionStringBuilder service used to generate the connection string once</param>
    /// <param name="modificationService">The ModificationService used to delete modifications when deleting an item during testing</param>
    public class ItemService(ISqlConnectionStringBuilder connectionStringBuilder, IModificationService modificationService, IWebHostEnvironment webHostEnvironment) : IItemService
    {
        private readonly IModificationService modificationService = modificationService; // Used for item+modification(s) retrieval or DELETE testing
        private readonly string connectionString = connectionStringBuilder.GenerateConnectionString(); // The MySQL connection string
        private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;

        /// <summary>
        /// Creates an item
        /// </summary>
        /// <param name="name">Name of the item <= 64 characters</param>
        /// <param name="description">Description of the item <= 1024 characters</param>
        /// <param name="price">Price of the item</param>
        /// <param name="stock">Stock of the item, -1 for unlimited</param>
        /// <param name="hidden">Hidden from the menu</param>
        /// <returns>The ID of the created item</returns>
        public int CreateItem(string name, string description, decimal price, int stock, bool hidden)
        {
            string sqlStatement = $"INSERT INTO items (name, description, price, stock, hidden) VALUES (@name, @description, @price, @stock, @hidden)";

            int createdItemId = -1;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, creating item");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@name", MySqlDbType.VarChar)).Value = name;
                    command.Parameters.Add(new MySqlParameter("@description", MySqlDbType.VarChar)).Value = description;
                    command.Parameters.Add(new MySqlParameter("@price", MySqlDbType.Decimal)).Value = price;
                    command.Parameters.Add(new MySqlParameter("@stock", MySqlDbType.Int32)).Value = stock;
                    command.Parameters.Add(new MySqlParameter("@hidden", MySqlDbType.Bool)).Value = hidden;

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows == 1)
                    {
                        sqlStatement = "SELECT MAX(id) FROM items";
                        command = new MySqlCommand(sqlStatement, connection);

                        object? lastItemId = command.ExecuteScalar();

                        if (lastItemId != null)
                        {
                            createdItemId = (int)lastItemId;
                            System.Diagnostics.Debug.WriteLine($"Created item ID: {createdItemId}");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Created item: {(affectedRows == 1 ? "True" : "False")}");

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return createdItemId;
        }

        /// <summary>
        /// Add an item to the order using the default modifications
        /// </summary>
        /// <param name="orders_id">The ID of the order</param>
        /// <param name="items_id">The ID of the item</param>
        /// <returns>The ID of the created order item. -1 for failure.</returns>
        public int AddOrderItem(int orders_id, int items_id, int quantity, List<int>? modificationIds, bool useDefaultModifications)
        {
            string sqlStatement = $"INSERT INTO order_items (quantity, orders_id, items_id) VALUES ({quantity}, {orders_id}, {items_id})";

            int createdItemId = -1;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, creating order item");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    int affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Created order item: {(affectedRows == 1 ? "True" : "False")}");

                    if (affectedRows == 1)
                    {
                        sqlStatement = "SELECT MAX(id) FROM order_items";
                        command = new MySqlCommand(sqlStatement, connection);

                        object? lastItemId = command.ExecuteScalar();

                        if (lastItemId != null)
                        {
                            createdItemId = (int)lastItemId;

                            if (useDefaultModifications)
                            {
                                List<ModificationModel> modifications = [.. modificationService.GetModificationsByItemId(items_id).Where(x => x.defaultModification)];
                                foreach (ModificationModel modification in modifications)
                                    modificationService.AddOrderModification(createdItemId, modification.id);
                            }
                            else if (modificationIds != null)
                            {
                                foreach (int modificationId in modificationIds)
                                    modificationService.AddOrderModification(createdItemId, modificationId);
                            }
                        }
                    }

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return createdItemId;
        }

        /// <summary>
        /// Update item in order
        /// </summary>
        /// <param name="orderItemId">The Order Item ID</param>
        /// <param name="quantity">The quantity of the item</param>
        /// <param name="modificationIds">The list of Modification IDs applied to the item</param>
        /// <returns></returns>
        public int UpdateOrderItem(int orderItemId, int quantity, List<int> modificationIds)
        {
            string sqlStatement = $"UPDATE order_items SET quantity = {quantity} WHERE id = {orderItemId}";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    ///
                    // UPDATE Order Item
                    ///
                    System.Diagnostics.Debug.WriteLine($"Connection to MySQL successful, updating order item with ID: {orderItemId}, Quantity: {quantity}, and Modifications: {string.Join(',', modificationIds)}");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Updated order item: {(affectedRows == 1 ? "True" : "False")}");

                    ///
                    // UPDATE Order Item Modifications
                    ///
                    sqlStatement = $"SELECT * FROM order_modifications WHERE order_items_id = {orderItemId}"; // Get existing modifications

                    HashSet<int> existingModificationSet = new HashSet<int>(); // For lookup of existing modifications
                    List<Tuple<int, int>> existingModifications = new List<Tuple<int, int>>(); // For recording existing modifications, both Order Modification ID and Modification ID

                    command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        existingModificationSet.Add(reader.GetInt32(1)); // Record existing Modification ID for lookup
                        existingModifications.Add(Tuple.Create<int, int>(reader.GetInt32(0), reader.GetInt32(1))); // Record existing Order Modification ID and Modification ID
                    }
                    reader.Close();

                    foreach (int modificationId in modificationIds) // Compare requested modifications
                    {
                        if (!existingModificationSet.Contains(modificationId)) // If not applied
                            modificationService.AddOrderModification(orderItemId, modificationId); // Apply modification
                    }

                    foreach (Tuple<int, int> modification in existingModifications) // Compare existing modifications
                    {
                        if (!modificationIds.Contains(modification.Item2)) // If not requested
                            modificationService.DeleteOrderModification(modification.Item1); // Remove modification
                    }

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
        /// Gets a list of all items
        /// </summary>
        /// <param name="onlyAvailableItems">Only return items that are not hidden and are in stock</param>
        /// <returns>A list of all items using the availability filter if requested</returns>
        public List<ItemModel> GetItems(bool onlyAvailableItems)
        {
            string sqlStatement = $"SELECT * FROM items{(onlyAvailableItems ? " WHERE (stock = -1 OR stock > 0) AND hidden = 0" : "")}";

            List<ItemModel> list = new List<ItemModel>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving items");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(ParseReaderToItemModel(reader));

                        System.Diagnostics.Debug.WriteLine($"Found item: {list.Last().id}:{list.Last().name}");
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

            return list;
        }

        /// <summary>
        /// Get a specific item by ID
        /// </summary>
        /// <param name="id">The ID of the requested item</param>
        /// <returns>ItemModel retrieved from the database. Null if not found</returns>
        public ItemModel? GetItem(int id)
        {
            ItemModel? itemModel = null;

            string sqlStatement = "SELECT * FROM items WHERE id = @id";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, finding item by id");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)).Value = id;

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        itemModel = ParseReaderToItemModel(reader);

                        System.Diagnostics.Debug.WriteLine($"Found item with {itemModel.id}:{itemModel.name}");
                    }
                    else
                        System.Diagnostics.Debug.WriteLine($"Failed to find item with id: {id}");
                    reader.Close();

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return itemModel;
        }

        /// <summary>
        /// Updates an item with
        /// </summary>
        /// <param name="item">The updated item</param>
        /// <returns></returns>
        public int UpdateItem(ItemModel item)
        {
            ItemModel? originalItem = GetItem(item.id);

            string sqlStatement = $"UPDATE items SET name = @name, description = @description, price = @price, stock = @stock, hidden = @hidden WHERE id = @id";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, updating item");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@name", MySqlDbType.VarChar)).Value = item.name;
                    command.Parameters.Add(new MySqlParameter("@description", MySqlDbType.VarChar)).Value = item.description;
                    command.Parameters.Add(new MySqlParameter("@price", MySqlDbType.Decimal)).Value = item.price;
                    command.Parameters.Add(new MySqlParameter("@stock", MySqlDbType.Int32)).Value = item.stock;
                    command.Parameters.Add(new MySqlParameter("@hidden", MySqlDbType.Bool)).Value = item.hidden;
                    command.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)).Value = item.id;

                    affectedRows = command.ExecuteNonQuery();

                    if (affectedRows == 1 && originalItem != null && File.Exists(originalItem.imageLocation))
                    {
                        File.Move(originalItem.imageLocation, item.imageLocation);
                    }

                    System.Diagnostics.Debug.WriteLine($"Updated item: {(affectedRows == 1 ? "True" : "False")}");

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
        /// Deletes an item
        /// NOTE: For testing purposes only!
        /// </summary>
        /// <param name="id">The ID of the item to delete</param>
        /// <returns>The number of rows affected. 1 for success, 0 for failure</returns>
        public int DeleteItem(int id)
        {
            int affectedRows = 0;

            string sqlStatement = $"DELETE FROM items WHERE id = @id";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting item");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)).Value = id;

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted item: {(affectedRows == 1 ? "True" : "False")}");

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
        /// Remove an item from an order
        /// </summary>
        /// <param name="id">The ID of the order item</param>
        /// <returns></returns>
        public int DeleteOrderItem(int id)
        {
            string sqlStatement = $"DELETE FROM order_items WHERE id = {id}";

            int affectedRows = -1;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting order item");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted order item: {(affectedRows == 1 ? "True" : "False")}");

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
        /// Removes all items from an order for resetting the current order
        /// </summary>
        /// <param name="orderId">The ID of the order to remove items from</param>
        /// <returns></returns>
        public int DeleteAllOrderItemsByOrderId(int orderId)
        {
            string sqlStatement = $"DELETE FROM order_items WHERE orders_id = {orderId}";

            int affectedRows = -1;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting order items");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted order items: {(affectedRows > 0 ? "True" : "False")}");

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

        private static ItemModel ParseReaderToItemModel(MySqlDataReader reader)
        {
            return new ItemModel(
                reader.GetInt32(0), // id
                reader.GetString(1), // name
                reader.GetString(2), // description
                reader.GetDecimal(3), // price
                reader.GetInt32(4), // stock
                reader.GetBoolean(5)); // hidden
        }
    }
}
