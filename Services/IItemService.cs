using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    public interface IItemService
    {
        public int CreateItem(string name, string description, decimal price, int stock, bool hidden);

        public List<ItemModel> GetItems();

        public ItemModel GetItem(int id);

        public int UpdateItem(ItemModel item);

        public int DeleteItem(int id);
    }
}
