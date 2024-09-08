using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using System.Linq;

namespace OnlineShopWebApp.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        public ProductController() { }

        [Route("index/{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = new TempProductsStorage().GetAll()
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound($"Товар с ID:{id} не найден");

            var productInfo = $"{product.Id}\n{product.Name}\n{product.Cost}\n{product.Description}\n{product.Category}";

            var productSpecificationsText = product.Specifications.Select(spec => $"{spec.Key}: {spec.Value}");

            var requestAnswer = $"{productInfo}\n\nХарактеристики:\n{string.Join("\n",productSpecificationsText)}";

            return Ok(requestAnswer);
        }
    }
}
