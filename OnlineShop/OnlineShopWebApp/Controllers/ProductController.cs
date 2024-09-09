using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private ProductsService _products;

        public ProductController()
        {
            _products = new ProductsService();
        }

        [Route("index/{id}")]
        public IActionResult Get(Guid id)
        {
            return _products.TryGetInfo(id, out var info) ? Ok(info) : BadRequest(info);
        }
    }
}
