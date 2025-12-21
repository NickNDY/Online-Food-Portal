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
            return [.. orderService.GetOrders().Where(x => x.order.completed || x.order.cancelled)];
        }

        /// <summary>
        /// Returns all orders that are not completed
        /// </summary>
        /// <returns>A list of incomplete orders</returns>
        public List<OrderDTO> GetIncompleteOrders()
        {
            return [.. orderService.GetOrders().Where(x => !x.order.completed && !x.order.cancelled)];
        }

        public bool CancelOrder(int orderId)
        {
            OrderDTO? order = orderService.GetOrderDTO(orderId);

            if (order == null)
                return false;

            order.order.cancelled = true;
            order.order.date_placed = DateTime.Now;

            orderService.UpdateOrder(order.order);

            // Process refund

            return true;
        }

        public bool CompleteOrder(int orderId)
        {
            OrderDTO? order = orderService.GetOrderDTO(orderId);

            if (order == null)
                return false;

            order.order.completed = !order.order.completed;
            order.order.date_placed = DateTime.Now;

            orderService.UpdateOrder(order.order);

            return true;
        }

        public bool PickupOrder(int orderId)
        {
            OrderDTO? order = orderService.GetOrderDTO(orderId);

            if (order == null)
                return false;

            order.order.picked_up = !order.order.picked_up;
            order.order.date_placed = DateTime.Now;

            orderService.UpdateOrder(order.order);

            return true;
        }
    }
}
