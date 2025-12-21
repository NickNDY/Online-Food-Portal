using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Controllers
{
    [Route("User")]
    [Authorize(Roles = "User")]
    public class UserController(IItemService itemService, IModificationService modificationService, IUserService authService, IStoreSettingsService storeSettingsService, IOrderService orderService) : Controller
    {
        private readonly IItemService itemService = itemService;
        private readonly IModificationService modificationService = modificationService;
        private readonly IUserService authService = authService;
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

            return View(itemDTOs);
        }

        /// <summary>
        /// Item Modification Page
        /// </summary>
        /// <param name="itemId">The chosen item ID</param>
        /// <param name="orderItemId">The chosen orderItemId</param>
        /// <returns>Shows a page allowing modification of the chosen item before adding or updating to order</returns>
        [Route("ItemModification")]
        public IActionResult ItemModification()
        {
            int
                itemId = -1,
                orderItemId = -1;

            string?
                itemIdString = Request.Query["itemId"], // Check for ?itemId=x
                orderItemIdString = Request.Query["orderItemId"]; // Check for ?orderItemId=x

            if (orderItemIdString != null && int.TryParse(orderItemIdString, out orderItemId)) // Modify existing order item
            {
                HashSet<int> setModifications = new HashSet<int>(); // Modifications applied to item
                OrderItemDTO? orderItem = orderService.GetOrderItem(orderItemId); // Get Order Item & Modifications in database

                if (orderItem == null) return NotFound(); // Item not found

                orderItem.OrderModificationModels.ForEach(x => setModifications.Add(x.modifications_id)); // Note all modifications applied to item

                orderItem.OrderModificationModels.AddRange( // Add applicable modifications
                    modificationService.GetModificationsByItemId(orderItem.OrderItemModel.itemModel.id) // Get all applicable modifications
                    .Where(x => setModifications.Add(x.id)) // Filter by setModifications.add = true
                    .Select(x => new OrderModificationModel(-1, x.id, orderItemId, false, x))); // Transform to OrderModificationModel

                List<OrderModificationModel> sortedList = orderItem.OrderModificationModels.OrderBy(x => x.modificationModel.id).ToList();
                orderItem.OrderModificationModels.Clear();
                orderItem.OrderModificationModels.AddRange(sortedList);

                return View(orderItem);
            }

            if (itemIdString != null && int.TryParse(itemIdString, out itemId)) // Modify new order item
            {
                ItemModel? itemModel = itemService.GetItem(itemId); // Get Item

                if (itemModel == null) return NotFound(); // Item not found

                
                List<OrderModificationModel> modifications = // List applicable modifications
                    [.. modificationService.GetModificationsByItemId(itemModel.id) // Get all applicable modifications
                    .Select(x => new OrderModificationModel(-1, x.id, -1, x.defaultModification, x))]; // Transform to OrderModificationModel

                return View(new OrderItemDTO(new OrderItemModel(-1, 1, -1, itemModel.id, itemModel), modifications)); // Transform to OrderItemDTO
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

        [Route("CartPartial")]
        public IActionResult CartPartial()
        {
            if (!GetUser(out UserModel? user) || user == null)
                return Content("<p>Please login to see cart</p>");

            OrderDTO? currentOrder = orderService.GetOrderDTO(orderService.GetCurrentOrder(user.id).id);

            if (currentOrder == null) return Content("<p>Cart not found</p>");

            return PartialView("_CartPartial", currentOrder);
        }

        /// <summary>
        /// Submit Order route
        /// </summary>
        /// <returns>Redirects to the user orders page if order is found and has items to submit</returns>
        [Route("SubmitOrder")]
        public IActionResult SubmitOrder()
        {
            if (!GetUser(out UserModel? user) || user == null)
                return Redirect("/Identity/Account/Login");

            OrderDTO? currentOrder = orderService.GetOrderDTO(orderService.GetCurrentOrder(user.id).id);

            if (currentOrder == null) return NotFound();

            if (currentOrder.items.Count == 0) return RedirectToAction("Menu", "User");

            currentOrder.order.submitted = true;
            currentOrder.order.date_placed = DateTime.Now;
            currentOrder.order.subtotal = currentOrder.subtotal;

            orderService.UpdateOrder(currentOrder.order);

            return RedirectToAction("Account", "User");
        }

        [Route("CancelOrder")]
        public IActionResult CancelOrder()
        {
            if (!GetUser(out UserModel? user) || user == null)
                return Redirect("/Identity/Account/Login");

            OrderModel currentOrder = orderService.GetCurrentOrder(user.id);

            itemService.DeleteAllOrderItemsByOrderId(currentOrder.id);

            return RedirectToAction("Cart", "User");
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

            List<OrderModel> orders = orderService.GetOrdersByUserId(user.id).Where(x => x.submitted).ToList();

            orders.Reverse();

            return View(orders);
        }

        [Route("PartialOrder/{orderId}")]
        public IActionResult GetPartialCart(int orderId)
        {
            OrderModel? order = orderService.GetOrder(orderId);

            if (order == null)
                return NotFound();

            return PartialView("_OrderPartial", order);
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
