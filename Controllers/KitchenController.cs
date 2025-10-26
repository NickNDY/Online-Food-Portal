using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Online_Food_Portal.Controllers
{
    [Route("Kitchen")]
    [Authorize(Roles = "Kitchen")]
    public class KitchenController : Controller
    {
        /// <summary>
        /// Orders In Progress Page
        /// </summary>
        /// <returns>The view for orders in progress</returns>
        [Route("Home")]
        public IActionResult Home()
        {
            return View();
        }

        /// <summary>
        /// Selected Order Page
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>The view for a selected order</returns>
        [Route("OrderSelect")]
        public IActionResult OrderSelect(int orderId)
        {
            return View();
        }

        /// <summary>
        /// Past Orders Page
        /// </summary>
        /// <returns>The view for completed orders</returns>
        [Route("PastOrders")]
        public IActionResult PastOrders()
        {
            return View();
        }
    }
}
