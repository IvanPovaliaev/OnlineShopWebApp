using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;

namespace OnlineShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private ProductsService _productsService;
        public HomeController()
        {
            _productsService = new ProductsService();
        }

        public IActionResult Index()
        {
            var products = _productsService.GetAll();

            return View(products);
        }
    }
}
