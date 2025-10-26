using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Services;

namespace Online_Food_Portal.Controllers
{
    [Route("User")]
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        /// <summary>
        /// Home Page
        /// </summary>
        /// <returns>Shows the home page for users displaying store information</returns>
        [Route("Home")]
        public IActionResult Home()
        {
            return View();
        }

        /// <summary>
        /// Menu Page
        /// </summary>
        /// <returns>Shows the menu page for users to order from</returns>
        [Route("Menu")]
        public IActionResult Menu()
        {
            return View();
        }

        /// <summary>
        /// Item Modification Page
        /// </summary>
        /// <param name="itemId">The chosen item ID</param>
        /// <returns>Shows a page allowing modification of the chosen item before adding to cart</returns>
        [Route("ItemModification")]
        public IActionResult ItemModification(int itemId)
        {
            return View();
        }

        /// <summary>
        /// Checkout Page
        /// </summary>
        /// <returns>Shows the cart contents and allows checking out</returns>
        [Route("Cart")]
        public IActionResult Cart()
        {
            return View();
        }

        /// <summary>
        /// Account Page
        /// </summary>
        /// <returns>Shows past and in progress orders</returns>
        [Route("Account")]
        public IActionResult Account()
        {
            return View();
        }
    }
}
