using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// User specific data service
    /// 
    /// Connects services to facilitate user ordering
    /// </summary>
    /// <param name="orderService">The order service used to find and update user orders</param>
    /// <param name="itemService">The item service used to find and update items in user orders</param>
    public class UserService(IOrderService orderService, IItemService itemService) : IUserService
    {
        private readonly IOrderService orderService = orderService;
        private readonly IItemService itemService = itemService;

        public bool AddItemToOrder(int userId, int itemId)
        {
            OrderModel order = orderService.GetCurrentOrder(userId);

            return itemService.AddOrderItem(order.id, itemId) == 1;
        }

        public List<OrderDTO> GetOrders(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
