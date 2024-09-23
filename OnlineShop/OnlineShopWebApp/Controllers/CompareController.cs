using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;
using System.Linq;

namespace OnlineShopWebApp.Controllers
{
    public class CompareController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly ProductsService _productsService;
        private readonly CartsService _cartsService;

        public CompareController(ProductsService productsService, CartsService cartsService)
        {
            _productsService = productsService;
            _cartsService = cartsService;
        }

        /// <summary>
        /// Open compare page
        /// </summary>
        /// <returns>Compare page View</returns>
        public IActionResult Index()
        {
            var products = _productsService.GetAll();
            var groupProducts = products.ToLookup(pr => pr.Category);

            return View(groupProducts);
        }
    }
}
