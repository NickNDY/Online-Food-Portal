using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    public interface IModificationService
    {
        public int CreateModification(string name, string description, decimal price_offset, int stock, bool defaultModification, bool hidden, int items_id);

        public List<ModificationModel> GetModifications();

        public List<ModificationModel> GetModifications(int items_id);

        public ModificationModel GetModification(int id);

        public int UpdateModification(ModificationModel model);

        public int DeleteModification(int id);
    }
}
