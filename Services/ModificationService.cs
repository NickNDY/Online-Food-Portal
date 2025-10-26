using Online_Food_Portal.Models;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Item Modification data service
    /// </summary>
    public class ModificationService : IModificationService
    {
        public int CreateModification(string name, string description, decimal price_offset, int stock, bool defaultModification, bool hidden, int items_id)
        {
            throw new NotImplementedException();
        }

        public List<ModificationModel> GetModifications()
        {
            throw new NotImplementedException();
        }

        public List<ModificationModel> GetModifications(int items_id)
        {
            throw new NotImplementedException();
        }

        public ModificationModel GetModification(int id)
        {
            throw new NotImplementedException();
        }

        public int UpdateModification(ModificationModel model)
        {
            throw new NotImplementedException();
        }

        public int DeleteModification(int id)
        {
            throw new NotImplementedException();
        }
    }
}
