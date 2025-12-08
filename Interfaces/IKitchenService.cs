using Online_Food_Portal.Models;

namespace Online_Food_Portal.Interfaces
{
    public interface IKitchenService
    {
        public List<OrderDTO> GetIncompleteOrders();

        public List<OrderDTO> GetCompleteOrders();

        public bool CompleteOrder(int orderId);

        public bool PickupOrder(int orderId);

        public bool CancelOrder(int orderId);
    }
}
