using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Item data service
    /// 
    /// Upon testing the user service, it became apparent that the SQL client previously used was incompatible with the MySQL database in use
    /// Much of the services were reset for redevelopment
    /// </summary>
    public class ItemService : IItemService
    {
        public int CreateItem(string name, string description, decimal price, int stock, bool hidden)
        {
            throw new NotImplementedException();
        }

        public List<ItemModel> GetItems()
        {
            throw new NotImplementedException();
        }

        public ItemModel GetItem(int id)
        {
            throw new NotImplementedException();
        }

        public int UpdateItem(ItemModel item)
        {
            throw new NotImplementedException();
        }

        public int DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}
