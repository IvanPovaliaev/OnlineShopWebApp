using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductsService _productsService;

        public HomeController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Get all product for Home Page
        /// </summary>
        /// <returns>Home page View</returns>
        public async Task<IActionResult> Index()
        {
            var products = await _productsService.GetAllAsync();

            return View(products);
        }

        /// <summary>
        /// Displays the About page View.
        /// </summary>
        /// <returns>About page View</returns>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Displays the Contacts page View.
        /// </summary>
        /// <returns>Contacts page View</returns>
        public IActionResult Contacts()
        {
            return View();
        }

        /// <summary>
        /// Displays the Delivery page View.
        /// </summary>
        /// <returns>Delivery page View</returns>
        public IActionResult Delivery()
        {
            return View();
        }

        /// <summary>
        /// Display Error View
        /// </summary>
        /// <returns>Error </returns>
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }

            return View();
        }
    }
}
