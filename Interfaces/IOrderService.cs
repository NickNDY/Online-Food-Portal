using Online_Food_Portal.Models;

namespace Online_Food_Portal.Interfaces
{
    public interface IOrderService
    {
        public int CreateOrder(int userId);

        public List<OrderModel> GetOrdersByUserId(int userId);

        public List<OrderDTO> GetOrders();

        public OrderModel? GetOrder(int orderId);

        public OrderDTO? GetOrderDTO(int id);

        public OrderItemDTO? GetOrderItem(int order_items_id);

        public OrderModel GetCurrentOrder(int userId);

        public int UpdateOrder(OrderModel orderModel);

        public int DeleteOrder(int id);
    }
}
