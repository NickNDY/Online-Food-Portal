using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;
using System.Net;

namespace Online_Food_Portal.API
{
    [Route("api/User")]
    [ApiController]
    public class UserAPI(IItemService itemService, IOrderService orderService, IUserService userService) : ControllerBase
    {
        private readonly IItemService itemService = itemService;
        private readonly IOrderService orderService = orderService;
        private readonly IUserService userService = userService;

        /// <summary>
        /// For passing a single number value with JSON
        /// </summary>
        public class SingleNumberModel
        {
            public int value { get; set; }
        }

        /// <summary>
        /// For passing a modified item with JSON
        /// </summary>
        public class ModifiedItemModel
        {
            public int id { get; set; }
            public int orderItemId { get; set; }
            public int quantity { get; set; }
            public List<int> modifications { get; set; } = new List<int>();
        }

        /// <summary>
        /// Add single item to order by Item ID
        /// </summary>
        /// <param name="item">JSON object containing the Item ID as value</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddItemToOrder")]
        public HttpResponseMessage AddItemToOrder([FromBody] SingleNumberModel item)
        {
            UserModel? user;
            if (!GetUser(out user) || user == null)
                return GenerateResponse(HttpStatusCode.Forbidden, "Please Login");

            System.Diagnostics.Debug.WriteLine($"Request to add item ID: {item.value} to order for user: {user.username}");

            ItemModel? itemModel = itemService.GetItem(item.value);

            if (itemModel == null)
                return GenerateResponse(HttpStatusCode.NotFound, "Not Found");

            OrderModel order = orderService.GetCurrentOrder(user.id);

            if (itemService.AddOrderItem(order.id, itemModel.id, 1, null, true) != -1)
            {
                System.Diagnostics.Debug.WriteLine($"Successfully added order item ID: {item.value} to order for user: {user.username}");
                return GenerateResponse(HttpStatusCode.OK, "Success");
            }
            else
                return GenerateResponse(HttpStatusCode.BadRequest, "Bad Request");
        }

        /// <summary>
        /// Remove single item from order by Order Item ID
        /// </summary>
        /// <param name="item">JSON object containing the Order Item ID as value</param>
        [HttpDelete]
        [Route("RemoveItemFromOrder")]
        public void RemoveItemFromOrder([FromBody] SingleNumberModel item)
        {
            UserModel? user;
            if (!GetUser(out user) || user == null)
                return;

            System.Diagnostics.Debug.WriteLine($"Request to remove order item ID: {item.value} from order for user: {user.username}");

            itemService.DeleteOrderItem(item.value);
        }

        /// <summary>
        /// Add or update modified item to order
        /// </summary>
        /// <param name="item">JSON object containing the modified item</param>
        /// <returns>Text message containing request results</returns>
        [HttpPost]
        [Route("AddModifiedItemToOrder")]
        public HttpResponseMessage AddModifiedItemToOrder([FromBody] ModifiedItemModel item)
        {
            UserModel? user;
            if (!GetUser(out user) || user == null)
                return GenerateResponse(HttpStatusCode.Forbidden, "Please Login");

            ItemModel? itemModel = itemService.GetItem(item.id);

            if (itemModel == null)
                return GenerateResponse(HttpStatusCode.NotFound, "Not Found");

            OrderModel order = orderService.GetCurrentOrder(user.id);

            if (item.orderItemId == -1) // Add item to order
            {
                if (itemService.AddOrderItem(order.id, item.id, item.quantity, item.modifications, false) != -1)
                    return GenerateResponse(HttpStatusCode.OK, "Success");
            }
            else // Update item in order
            {
                if (itemService.UpdateOrderItem(item.orderItemId, item.quantity, item.modifications) == 1)
                    return GenerateResponse(HttpStatusCode.OK, "Success");
            }

            return GenerateResponse(HttpStatusCode.BadRequest, "Bad Request");
        }

        private HttpResponseMessage GenerateResponse(HttpStatusCode code, string content)
        {
            HttpResponseMessage message = new HttpResponseMessage(code);

            message.ReasonPhrase = content;

            return message;
        }

        private bool GetUser(out UserModel? userModel)
        {
            userModel = null;

            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return false;

            string? username = User.Identity.Name;
            if (username == null)
                return false;

            UserModel? user = userService.GetUserByUsername(username);
            if (user == null)
                return false;

            userModel = user;
            return true;
        }
    }
}
