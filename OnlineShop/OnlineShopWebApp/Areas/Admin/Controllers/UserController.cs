using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        /// <summary>
        /// Open Admin Users Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
