using Online_Food_Portal.Models;

namespace Online_Food_Portal.Interfaces
{
    public interface IItemService
    {
        public int CreateItem(string name, string description, decimal price, int stock, bool hidden);

        public int AddOrderItem(int orders_id, int items_id, int quantity, List<int>? modificationIds, bool useDefaultModifications);

        public List<ItemModel> GetItems(bool onlyAvailableItems);

        public ItemModel? GetItem(int id);

        public int UpdateItem(ItemModel item);

        public int UpdateOrderItem(int orderItemId, int quantity, List<int> modificationIds);

        public int DeleteItem(int id);

        public int DeleteOrderItem(int id);

        public int DeleteAllOrderItemsByOrderId(int orderId);
    }
}
