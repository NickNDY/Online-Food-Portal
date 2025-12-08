using Online_Food_Portal.Models;

namespace Online_Food_Portal.Interfaces
{
    public interface IUserService
    {
        public List<OrderDTO> GetOrders(int userId);

        public bool AddItemToOrder(int userId, int itemId);
    }
}
