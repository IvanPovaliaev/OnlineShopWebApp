using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class CartController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private CartsService _cartsService;
        private ProductsService _productsService;

        public CartController()
        {
            _cartsService = new CartsService();
            _productsService = new ProductsService();
        }

        public IActionResult Index()
        {
            var cart = _cartsService.Get(_userId);
            return View(cart);
        }

        public IActionResult Add(Guid productId)
        {
            var product = _productsService.Get(productId);
            _cartsService.Add(product, _userId);
            return RedirectToAction("Index");
        }
    }
}
