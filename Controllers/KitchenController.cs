using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Controllers
{
    [Route("Kitchen")]
    [Authorize(Roles = "Kitchen")]
    public class KitchenController : Controller
    {
        private readonly IKitchenService kitchenService;
        private readonly IOrderService orderService;

        public KitchenController(IKitchenService kitchenService, IOrderService orderService)
        {
            this.kitchenService = kitchenService;
            this.orderService = orderService;
        }

        /// <summary>
        /// Orders In Progress Page
        /// </summary>
        /// <returns>The view for orders in progress</returns>
        [Route("Home")]
        public IActionResult Home()
        {
            return View(kitchenService.GetIncompleteOrders());
        }

        /// <summary>
        /// Selected Order Page
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>The view for a selected order</returns>
        [Route("OrderSelect")]
        public IActionResult OrderSelect(int orderId)
        {
            OrderDTO? order = orderService.GetOrderDTO(orderId);

            if (order == null)
                return RedirectToAction("Home", "KitchenController");

            return View(order);
        }

        /// <summary>
        /// Past Orders Page
        /// </summary>
        /// <returns>The view for completed orders</returns>
        [Route("PastOrders")]
        public IActionResult PastOrders()
        {
            return View(kitchenService.GetCompleteOrders());
        }
    }
}
