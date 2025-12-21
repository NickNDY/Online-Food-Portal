using MySqlConnector;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Order data service
    /// 
    /// Handles order specific data transactions
    /// </summary>
    /// <param name="itemService">Item service for finding items for order items</param>
    /// <param name="modificationService">Modification service for finding modifications for order modifications</param>
    /// <param name="connectionStringBuilder">The SqlConnectionStringBuilder service used to generate the connection string once</param>
    public class OrderService(IItemService itemService, IModificationService modificationService, ISqlConnectionStringBuilder connectionStringBuilder) : IOrderService
    {
        private readonly IItemService itemService = itemService;
        private readonly IModificationService modificationService = modificationService;
        private readonly string connectionString = connectionStringBuilder.GenerateConnectionString();

        /// <summary>
        /// Creates an empty order for the specified user
        /// </summary>
        /// <param name="userId">The ID of the specified user</param>
        /// <returns>The order ID of the created order</returns>
        public int CreateOrder(int userId)
        {
            string sqlStatement = $"INSERT INTO orders (subtotal, date_placed, submitted, cancelled, completed, picked_up, users_id) VALUES (@subtotal, @date_placed, @submitted, @cancelled, @completed, @picked_up, @users_id)";

            int createdOrderId = -1;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, creating order");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@subtotal", MySqlDbType.Decimal)).Value = 0;
                    command.Parameters.Add(new MySqlParameter("@date_placed", MySqlDbType.DateTime)).Value = DateTime.Now;
                    command.Parameters.Add(new MySqlParameter("@submitted", MySqlDbType.Int16)).Value = false;
                    command.Parameters.Add(new MySqlParameter("@cancelled", MySqlDbType.Int16)).Value = false;
                    command.Parameters.Add(new MySqlParameter("@completed", MySqlDbType.Int16)).Value = false;
                    command.Parameters.Add(new MySqlParameter("@picked_up", MySqlDbType.Int16)).Value = false;
                    command.Parameters.Add(new MySqlParameter("@users_id", MySqlDbType.Int32)).Value = userId;

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows == 1)
                    {
                        sqlStatement = "SELECT MAX(id) FROM orders";
                        command = new MySqlCommand(sqlStatement, connection);

                        object? lastOrderId = command.ExecuteScalar();

                        if (lastOrderId != null)
                        {
                            createdOrderId = (int)lastOrderId;
                            System.Diagnostics.Debug.WriteLine($"Created order ID: {createdOrderId}");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Created order: {(affectedRows == 1 ? "True" : "False")}");

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return createdOrderId;
        }

        /// <summary>
        /// Returns all orders
        /// </summary>
        /// <returns>A list of all orders</returns>
        public List<OrderDTO> GetOrders()
        {
            string sqlStatement = "SELECT * FROM orders";

            List<OrderDTO> list = new List<OrderDTO>();
            List<OrderModel> orders = new List<OrderModel>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving orders");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        orders.Add(ParseReaderToOrderModel(reader));

                        System.Diagnostics.Debug.WriteLine($"Found order: {orders.Last().id}");
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

            foreach (OrderModel model in orders)
                list.Add(GetOrderDTO(model));

            return list;
        }

        public OrderModel? GetOrder(int orderId)
        {
            string sqlStatement = $"SELECT * FROM orders WHERE id = {orderId}";

            OrderModel? order = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving order");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                        order = ParseReaderToOrderModel(reader);
                    reader.Close();

                    System.Diagnostics.Debug.WriteLine($"Found order: {order != null}");

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return order;
        }

        /// <summary>
        /// Returns a specific order
        /// </summary>
        /// <param name="id">The specified order ID</param>
        /// <returns>The requested order, or null if not found</returns>
        public OrderDTO? GetOrderDTO(int id)
        {
            OrderModel? order = GetOrder(id);

            return order != null ? GetOrderDTO(order) : null;
        }

        /// <summary>
        /// Find all the order items for the requested order turning the order model into an order DTO
        /// </summary>
        /// <param name="order">The order model to find items for</param>
        /// <returns>The order DTO containing all the items and modifications relevant to the order</returns>
        private OrderDTO GetOrderDTO(OrderModel order)
        {
            OrderDTO orderDTO = new OrderDTO(order, GetOrderItems(order.id));

            return orderDTO;
        }

        /// <summary>
        /// Returns a list of all order items for the specified order
        /// </summary>
        /// <param name="orders_id">The ID of the specified order</param>
        /// <returns>A list of all the items in the order and their respective modifications</returns>
        private List<OrderItemDTO> GetOrderItems(int orders_id)
        {
            List<OrderItemModel> orderItemModels = new List<OrderItemModel>();

            string sqlStatement =
                "SELECT * FROM order_items " +
                $"LEFT JOIN items ON order_items.items_id = items.id " +
                $"WHERE order_items.orders_id = {orders_id}";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving order items");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        orderItemModels.Add(ParseReaderToOrderItemModel(reader));
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

            List<OrderItemDTO> orderItemDTOs = new List<OrderItemDTO>();

            foreach (OrderItemModel orderItemModel in orderItemModels)
            {
                orderItemDTOs.Add(new OrderItemDTO(orderItemModel, GetOrderItemModifications(orderItemModel.id)));
            }

            return orderItemDTOs;
        }

        /// <summary>
        /// Gets a specific order item and all of its' modifications
        /// </summary>
        /// <param name="order_items_id">The ID of the specified order item</param>
        /// <returns>A DTO containing the order item and all of it's modifications</returns>
        public OrderItemDTO? GetOrderItem(int order_items_id)
        {
            OrderItemModel? orderItemModel = null;

            string sqlStatement =
                "SELECT * FROM order_items " +
                $"LEFT JOIN items ON order_items.items_id = items.id " +
                $"WHERE order_items.id = {order_items_id}";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving order items");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                        orderItemModel = ParseReaderToOrderItemModel(reader);
                    reader.Close();

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            return orderItemModel != null ? new OrderItemDTO(orderItemModel, GetOrderItemModifications(orderItemModel.id)) : null;
        }

        /// <summary>
        /// Gets all the modifications for a specific order item
        /// </summary>
        /// <param name="order_items_id">The ID of the specified order item</param>
        /// <returns>A list of all the modifications for the specified order item</returns>
        private List<OrderModificationModel> GetOrderItemModifications(int order_items_id)
        {
            List<OrderModificationModel> orderModificationModels = new List<OrderModificationModel>();

            string sqlStatement =
                "SELECT * FROM order_modifications " +
                $"LEFT JOIN modifications ON order_modifications.modifications_id = modifications.id " +
                $"WHERE order_items_id = {order_items_id}";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving order item modifications");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        OrderModificationModel orderModificationModel = ParseReaderToOrderModificationModel(reader);
                        orderModificationModel.setModification = true;
                        orderModificationModels.Add(orderModificationModel);
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

            return orderModificationModels;
        }

        /// <summary>
        /// Returns the current order for the specified user
        /// Only returns the currently not-submitted order
        /// If one is not found, it is created
        /// </summary>
        /// <param name="userId">The ID of the specified user</param>
        /// <returns></returns>
        public OrderModel GetCurrentOrder(int userId)
        {
            string sqlStatement = $"SELECT * FROM orders WHERE submitted = 0 AND users_id = @users_id";

            OrderModel? order = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine($"Connection to MySQL successful, retrieving current order for user {userId}");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@users_id", MySqlDbType.Int32)).Value = userId;

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                        order = ParseReaderToOrderModel(reader);
                    reader.Close();

                    System.Diagnostics.Debug.WriteLine($"Found order: {order != null}");

                    connection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection to MySQL closed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to SQL Database: {ex.Message}");
            }

            if (order == null) // Order not found, create a new order
            {
                CreateOrder(userId);

                return GetCurrentOrder(userId);
            }

            return order;
        }

        public List<OrderModel> GetOrdersByUserId(int users_id)
        {
            string sqlStatement = $"SELECT * FROM orders WHERE users_id = {users_id}";

            List<OrderModel> orders = new List<OrderModel>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, retrieving orders");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    MySqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        orders.Add(ParseReaderToOrderModel(reader));

                        System.Diagnostics.Debug.WriteLine($"Found order: {orders.Last().id}");
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

            return orders;
        }

        public int UpdateOrder(OrderModel orderModel)
        {
            OrderDTO? orderDTO = GetOrderDTO(orderModel.id);

            if (orderDTO == null) return 0;

            string sqlStatement =
                    "UPDATE orders SET " +
                    $"subtotal = {orderModel.subtotal}, " +
                    $"date_placed = @date_placed, " +
                    $"submitted = {(orderModel.submitted ? "1" : "0")}, " +
                    $"cancelled = {(orderModel.cancelled ? "1" : "0")}, " +
                    $"completed = {(orderModel.completed ? "1" : "0")}, " +
                    $"picked_up = {(orderModel.picked_up ? "1" : "0")} " +
                    $"WHERE id = {orderModel.id}";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, updating order");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.Add(new MySqlParameter("@date_placed", MySqlDbType.DateTime)).Value = orderModel.date_placed.AddMilliseconds(-orderModel.date_placed.Millisecond);

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Updated order: {(affectedRows == 1 ? "True" : "False")}");

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

        public int DeleteOrder(int id)
        {
            string sqlStatement = $"DELETE FROM orders WHERE id = {id}";

            int affectedRows = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine("Connection to MySQL successful, deleting order");

                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    affectedRows = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"Deleted order: {(affectedRows == 1 ? "True" : "False")}");

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

        private static OrderModel ParseReaderToOrderModel(MySqlDataReader reader)
        {
            return new OrderModel(
                reader.GetInt32(0),  // id
                reader.GetDecimal(1), // subtotal
                reader.GetDateTime(2), // time placed
                reader.GetBoolean(3), // submitted
                reader.GetBoolean(4), // cancelled
                reader.GetBoolean(5), // completed
                reader.GetBoolean(6), // picked up
                reader.GetInt32(7)); // user id
        }

        private static OrderItemModel ParseReaderToOrderItemModel(MySqlDataReader reader)
        {
            return new OrderItemModel(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3),
                new ItemModel(
                    reader.GetInt32(4),
                    reader.GetString(5),
                    reader.GetString(6),
                    reader.GetDecimal(7),
                    reader.GetInt32(8),
                    reader.GetBoolean(9)));
        }

        private static OrderModificationModel ParseReaderToOrderModificationModel(MySqlDataReader reader)
        {
            return new OrderModificationModel(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                false,
                new ModificationModel(
                    reader.GetInt32(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetDecimal(6),
                    reader.GetInt32(7),
                    reader.GetBoolean(8),
                    reader.GetBoolean(9),
                    reader.GetInt32(10)));
        }

        public int UpdateOrder(OrderDTO orderModel)
        {
            throw new NotImplementedException();
        }
    }
}
