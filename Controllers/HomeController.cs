using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Data;
using System.Security.Claims;
using System.Text;

namespace Online_Food_Portal.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Redirection controller for directing users/ kitchen/ administrators by role
        /// </summary>
        /// <returns>
        ///   User/Home for users, Kitchen/Home for kitchen, Administrative/Home for administrators
        ///   Shows a message prompting registration for unauthorized users
        /// </returns>
        [Route("Home")]
        public IActionResult Home()
        {
            if (User.IsInRole("User"))
                return Redirect("User/Home");

            if (User.IsInRole("Kitchen"))
                return Redirect("Kitchen/Home");

            if (User.IsInRole("Administrator"))
                return Redirect("Administrative/Home");

            return View();
        }
    }
}
