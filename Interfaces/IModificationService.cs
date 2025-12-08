using Online_Food_Portal.Models;

namespace Online_Food_Portal.Interfaces
{
    public interface IModificationService
    {
        public int CreateModification(string name, string description, decimal price_offset, int stock, bool defaultModification, bool hidden, int items_id);

        public int AddOrderModification(int order_items_id, int modifications_id);

        public List<ModificationModel> GetModifications();

        public List<ModificationModel> GetModificationsByItemId(int items_id);

        public ModificationModel? GetModification(int id);

        public int UpdateModification(ModificationModel model);

        public int DeleteModification(int id);

        public int DeleteModificationsByItemId(int items_id);

        public int DeleteOrderModification(int id);

        public int DeleteAllOrderModificationsByOrderItemId(int order_items_id);
    }
}
