using Online_Food_Portal.Models;

namespace Online_Food_Portal.Interfaces
{
    public interface IStoreSettingsService
    {
        public StoreSettingsModel GetStoreSettings();
        public int SetStoreSettings(StoreSettingsModel model);
    }
}
