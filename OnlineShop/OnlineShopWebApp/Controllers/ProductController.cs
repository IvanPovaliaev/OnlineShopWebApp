using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private ProductsService _productsService;

        public ProductController()
        {
            _productsService = new ProductsService();
        }

        [Route("index/{id}")]
        public IActionResult Get(Guid id)
        {
            return View(_productsService.Get(id));
        }
    }
}
