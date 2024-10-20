using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;

namespace OnlineShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private ProductsService _productsService;

        public HomeController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Get all product for Home Page
        /// </summary>
        /// <returns>Home page View</returns>
        public IActionResult Index()
        {
            var products = _productsService.GetAll();

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
