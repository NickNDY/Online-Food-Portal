using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Controllers
{
    [Route("Administrative")]
    [Authorize(Roles = "Administrator")]
    public class AdministrativeController : Controller
    {
        private readonly IItemService itemService;
        private readonly IModificationService modificationService;
        private readonly IStoreSettingsService storeSettingsService;

        public AdministrativeController(IItemService itemService, IStoreSettingsService storeSettingsService, IModificationService modificationService)
        {
            this.itemService = itemService;
            this.storeSettingsService = storeSettingsService;
            this.modificationService = modificationService;
        }

        /// <summary>
        /// Menu Page for Administrators
        /// </summary>
        /// <returns>The page showing the complete menu for administrative actions</returns>
        [Route("Home")]
        public IActionResult Home()
        {
            return View(itemService.GetItems(false));
        }

        /// <summary>
        /// Item Creation/Modification Page
        /// </summary>
        /// <param name="itemId">The ID of the item to modify, or -1 for item creation</param>
        /// <returns></returns>
        [Route("ItemCreation")]
        [HttpGet]
        public IActionResult ItemCreation(int itemId)
        {
            if (itemId == -1)
                return View();

            ItemModel? itemModel = itemService.GetItem(itemId);
            if (itemModel == null)
                return View();

            List<ModificationModel> modifications = modificationService.GetModificationsByItemId(itemId);

            ItemDTO itemDTO = new ItemDTO(itemModel, modifications);

            return View(itemDTO);
        }

        /// <summary>
        /// Item Creation/Modification Post route
        /// </summary>
        /// <param name="itemModel">The posted item model to create or update</param>
        /// <returns>Redirects to the administrative menu page</returns>
        [Route("ItemCreation")]
        [HttpPost]
        public IActionResult ItemCreation(ItemModel itemModel)
        {

            // Process created item
            return RedirectToAction("AdministrativeController", "Home");
        }

        /// <summary>
        /// Store Settings Page
        /// </summary>
        /// <returns>The page showing store settings</returns>
        [Route("StoreSettings")]
        [HttpGet]
        public IActionResult StoreSettings()
        {
            return View(storeSettingsService.GetStoreSettings());
        }

        /// <summary>
        /// Store Settings Update Post Page
        /// </summary>
        /// <param name="model">The Store Settings Model posted to the page</param>
        /// <returns>Redirects to the Administrative home page</returns>
        [Route("StoreSettings")]
        [HttpPost]
        public IActionResult StoreSettings(StoreSettingsModel model)
        {
            storeSettingsService.SetStoreSettings(model);
            return RedirectToAction("AdministrativeController", "Home");
        }
    }
}
