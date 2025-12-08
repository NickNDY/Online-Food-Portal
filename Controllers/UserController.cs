using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Controllers
{
    [Route("User")]
    [Authorize(Roles = "User")]
    public class UserController(IItemService itemService, IModificationService modificationService, IUserService userService, IAuthenticationService authService, IStoreSettingsService storeSettingsService, IOrderService orderService) : Controller
    {
        private readonly IItemService itemService = itemService;
        private readonly IModificationService modificationService = modificationService;
        private readonly IUserService userService = userService;
        private readonly IAuthenticationService authService = authService;
        private readonly IStoreSettingsService storeSettingsService = storeSettingsService;
        private readonly IOrderService orderService = orderService;

        /// <summary>
        /// Home Page
        /// </summary>
        /// <returns>Shows the home page for users displaying store information</returns>
        [Route("Home")]
        public IActionResult Home()
        {
            return View(storeSettingsService.GetStoreSettings());
        }

        /// <summary>
        /// Menu Page
        /// </summary>
        /// <returns>Shows the menu page for users to order from</returns>
        [Route("Menu")]
        public IActionResult Menu()
        {
            List<ItemModel> items = itemService.GetItems(true);

            List<ItemDTO> itemDTOs = new List<ItemDTO>();

            foreach (ItemModel item in items)
            {
                itemDTOs.Add(new ItemDTO(item, modificationService.GetModificationsByItemId(item.id)));
            }

            ViewBag.images = new Dictionary<int, string>();

            foreach (ItemModel item in items)
                ViewBag.images[item.id] = Path.Exists($"/images/{item.id}{item.name}.jpg") ? $"/images/{item.id}{item.name}.jpg" : "/images/itemplaceholder.jpg";

            return View(itemDTOs);
        }

        /// <summary>
        /// Item Modification Page
        /// </summary>
        /// <param name="itemId">The chosen item ID</param>
        /// <param name="orderItemId">The chosen orderItemId</param>
        /// <returns>Shows a page allowing modification of the chosen item before adding to cart</returns>
        [Route("ItemModification")]
        public IActionResult ItemModification()//, [FromQuery] int? _orderItemId
        {
            int
                itemId = -1,
                orderItemId = -1;

            string?
                itemIdString = Request.Query["itemId"],
                orderItemIdString = Request.Query["orderItemId"];

            if (int.TryParse(orderItemIdString, out orderItemId))
            {
                return View(orderService.GetOrderItem(orderItemId));
            }

            if (int.TryParse(itemIdString, out itemId))
            {
                ItemModel? itemModel = itemService.GetItem(itemId);

                if (itemModel == null) return NotFound();

                return View(new OrderItemDTO(new OrderItemModel(-1, 1, -1, itemModel.id, itemModel), [.. modificationService.GetModificationsByItemId(itemModel.id).Select(x => new OrderModificationModel(-1, -1, x.id, x))]));
            }

            return BadRequest();
        }

        /// <summary>
        /// Checkout Page
        /// </summary>
        /// <returns>Shows the cart contents and allows checking out</returns>
        [Route("Cart")]
        public IActionResult Cart()
        {
            if (!GetUser(out UserModel? user) || user == null)
                return Redirect("/Identity/Account/Login");

            return View(orderService.GetOrderDTO(orderService.GetCurrentOrder(user.id).id));
        }

        /// <summary>
        /// Account Page
        /// </summary>
        /// <returns>Shows past and in progress orders</returns>
        [Route("Account")]
        public IActionResult Account()
        {
            if (!GetUser(out UserModel? user) || user == null)
                return Redirect("/Identity/Account/Login");

            return View(userService.GetOrders(user.id));
        }

        private bool GetUser(out UserModel? userModel)
        {
            userModel = null;

            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return false;

            string? username = User.Identity.Name;
            if (username == null)
                return false;

            UserModel? user = authService.GetUserByUsername(username);
            if (user == null)
                return false;

            userModel = user;
            return true;
        }
    }
}
