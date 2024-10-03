using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductsService _productsService;

        public ProductController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <returns>Product page View</returns>
        /// <param name="id">Product id (guid)</param>
        public IActionResult Index(Guid id)
        {
            var product = _productsService.Get(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Get all products of the target category.
        /// </summary>
        /// <returns>Page View of all products of the specified category</returns>
        /// <param name="category">Product category</param>        
        public IActionResult Category(ProductCategories category)
        {
            var products = _productsService.GetAll(category);
            var productsWithCategory = (products, category);
            return View(productsWithCategory);
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>Page View of all products</returns>        
        public IActionResult All()
        {
            var products = _productsService.GetAll();
            return View(products);
        }

        /// <summary>
        /// Get all products that match the search query
        /// </summary>
        /// <returns>Page with found products</returns>        
        public IActionResult SearchResult(string searchQuery)
        {
            var products = _productsService.GetAllFromSearch(searchQuery);
            var productsWithQuery = (products, searchQuery);
            return View(productsWithQuery);
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <returns>Admins products View</returns> 
        /// <param name="product">Target product</param>
        [HttpPost]
        public IActionResult Add(Product product)
        {
            _productsService.Add(product);
            return RedirectToAction("Products", "Admin");
        }

        /// <summary>
        /// Delete product by Id
        /// </summary>
        /// <returns>Admins products View</returns>
        /// <param name="productId">Target productId</param>  
        public IActionResult Delete(Guid productId)
        {
            _productsService.Delete(productId);
            return RedirectToAction("Products", "Admin");
        }

        /// <summary>
        /// Update target product
        /// </summary>
        /// <returns>Admins products View</returns>
        [HttpPost]
        public IActionResult Update(Product product)
        {
            _productsService.Update(product);
            return RedirectToAction("Products", "Admin");
        }

        /// <summary>
        /// Get Partial View for product specifications depending on the category
        /// </summary>
        /// <returns>Relevant Partial View</returns>
        /// <param name="category">Product category</param>   
        public IActionResult GetSpecificationsForm(ProductCategories category)
        {
            return category switch
            {
                ProductCategories.GraphicCards => PartialView("~/Views/Admin/SpecificationsForms/_GraphicCardForm.cshtml"),
                ProductCategories.Processors => PartialView("~/Views/Admin/SpecificationsForms/_ProcessorForm.cshtml"),
                ProductCategories.Motherboards => PartialView("~/Views/Admin/SpecificationsForms/_MotherboardForm.cshtml"),
                ProductCategories.SSD => PartialView("~/Views/Admin/SpecificationsForms/_SsdForm.cshtml"),
                ProductCategories.HDD => PartialView("~/Views/Admin/SpecificationsForms/_HddForm.cshtml"),
                ProductCategories.RAM => PartialView("~/Views/Admin/SpecificationsForms/_RamForm.cshtml"),
                ProductCategories.PowerSupplies => PartialView("~/Views/Admin/SpecificationsForms/_PowerSupplyForm.cshtml"),
                _ => Content("No form available for this category.")
            };
        }
    }
}