using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;
using Online_Food_Portal.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Online_Food_Portal.API
{
    [Route("api/User")]
    [ApiController]
    [Produces("application/json")]
    public class UserAPI : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IItemService itemService;
        private readonly IAuthenticationService authService;

        public class SingleNumberModel
        {
            public int value { get; set; }
        }

        public UserAPI(IUserService userService, IItemService itemService, IAuthenticationService authService)
        {
            this.userService = userService;
            this.itemService = itemService;
            this.authService = authService;
        }

        // GET: api/<UserAPI>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserAPI>/5
        [HttpGet("{AddItemToOrder}")]
        public string Get([FromBody] int id)
        {
            return "id";
        }

        // POST api/<UserAPI>
        [HttpPost("{AddItemToOrder}")]
        public string AddItemToOrder([FromBody] SingleNumberModel item)
        {
            UserModel? user;
            if (!GetUser(out user) || user == null)
                return "Please Login";

            System.Diagnostics.Debug.WriteLine($"Request to add item ID: {item.value} to order for user: {user.username}");

            ItemModel? itemModel = itemService.GetItem(item.value);

            if (itemModel == null)
                return "Not Found";

            if (userService.AddItemToOrder(user.id, item.value))
            {
                System.Diagnostics.Debug.WriteLine($"Successfully added order item ID: {item.value} to order for user: {user.username}");
                return "Ok";
            }
            else
                return "Request Failed";
        }

        [HttpDelete("{RemoveItemFromOrder}")]
        public void RemoveItemFromOrder([FromBody] SingleNumberModel item)
        {
            UserModel? user;
            if (!GetUser(out user) || user == null)
                return;

            System.Diagnostics.Debug.WriteLine($"Request to remove order item ID: {item.value} from order for user: {user.username}");

            itemService.DeleteOrderItem(item.value);
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
