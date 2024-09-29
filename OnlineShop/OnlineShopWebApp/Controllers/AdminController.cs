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
	}
}
