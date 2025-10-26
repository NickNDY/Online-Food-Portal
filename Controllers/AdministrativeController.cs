using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Controllers
{
    [Route("Administrative")]
    [Authorize(Roles = "Administrator")]
    public class AdministrativeController : Controller
    {
        /// <summary>
        /// Menu Page for Administrators
        /// </summary>
        /// <returns>The page showing the complete menu for administrative actions</returns>
        [Route("Home")]
        public IActionResult Home()
        {
            return View();
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
            return View();
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
            return RedirectToAction("Home", "AdministrativeController");
        }

        /// <summary>
        /// Store Settings Page
        /// </summary>
        /// <returns>The page showing store settings</returns>
        [Route("StoreSettings")]
        [HttpGet]
        public IActionResult StoreSettings()
        {
            return View();
        }
    }
}
