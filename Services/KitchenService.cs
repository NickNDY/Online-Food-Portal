using MySqlConnector;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Kitchen service for facilitating kitchen specific functionality
    /// </summary>
    /// <param name="orderService">The order service for retrieving and updating orders and order statuses</param>
    public class KitchenService(IOrderService orderService) : IKitchenService
    {
        private readonly IOrderService orderService = orderService; // Order service for retrieving and updating orders

        /// <summary>
        /// Returns all orders that have been completed
        /// </summary>
        /// <returns>A list of complete orders</returns>
        public List<OrderDTO> GetCompleteOrders()
        {
            return [.. orderService.GetOrders().Where(x => x.order.completed)];
        }

        /// <summary>
        /// Returns all orders that are not completed
        /// </summary>
        /// <returns>A list of incomplete orders</returns>
        public List<OrderDTO> GetIncompleteOrders()
        {
            return [.. orderService.GetOrders().Where(x => !x.order.completed)];
        }

        public bool CancelOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public bool CompleteOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public bool PickupOrder(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
