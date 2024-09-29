using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApp.Controllers
{
	public class AdminController : Controller
	{
		/// <summary>
		/// Open Admin Order Page
		/// </summary>
		/// <returns>Admin Order View</returns>
		public IActionResult Order()
		{
			return View();
		}

		/// <summary>
		/// Open Admin Users Page
		/// </summary>
		/// <returns>Admin Users View</returns>
		public IActionResult Users()
		{
			return View();
		}

		/// <summary>
		/// Open Admin Roles Page
		/// </summary>
		/// <returns>Admin Roles View</returns>
		public IActionResult Roles()
		{
			return View();
		}

		/// <summary>
		/// Open Admin Products Page
		/// </summary>
		/// <returns>Admin Producs View</returns>
		public IActionResult Products()
		{
			return View();
		}
	}
}
