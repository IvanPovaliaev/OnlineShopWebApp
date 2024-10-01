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
        /// Delete product by Id
        /// </summary>
        /// <returns>Admins products View</returns>        
        public IActionResult Delete(Guid productId)
        {
            _productsService.Delete(productId);
            return RedirectToAction("Products", "Admin");
        }

        public IActionResult Create(Product product)
        {
            //string name, decimal cost, string description, ProductCategories category
            //var product = new Product(name, cost, description, category);
            //_productsService.Add(product);
            return RedirectToAction("Products", "Admin");
        }


        public IActionResult GetSpecificationsForm(ProductCategories category)
        {
            return category switch
            {
                ProductCategories.GraphicCards => PartialView("~/Views/Admin/SpecificationsForms/_GraphicCardForm.cshtml"),
                ProductCategories.Processors => PartialView("~/Views/Admin/SpecificationsForms/_ProcessorForm.cshtml"),
                ProductCategories.Motherboards => PartialView("~/Views/SpecificationsForms/Admin/_MotherboardForm.cshtml"),
                ProductCategories.SSD => PartialView("~/Views/Admin/SpecificationsForms/_SsdForm.cshtml"),
                ProductCategories.HDD => PartialView("~/Views/Admin/SpecificationsForms/_HddForm.cshtml"),
                ProductCategories.RAM => PartialView("~/Views/Admin/SpecificationsForms/_RamForm.cshtml"),
                ProductCategories.PowerSupplies => PartialView("~/Views/Admin/SpecificationsForms/_PowerSupplyForm.cshtml"),
                _ => Content("No form available for this category.")
            };
        }
    }
}