using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ProductsService _productsService;
        private readonly IExcelService _excelService;

        public ProductController(ProductsService productsService, IExcelService excelService)
        {
            _productsService = productsService;
            _excelService = excelService;
        }

        /// <summary>
        /// Open Admin Products Page
        /// </summary>
        /// <returns>Admin Products View</returns>
        public IActionResult Index()
        {
            var products = _productsService.GetAll();
            return View(products);
        }

        /// <summary>
        /// Open Admin AddProduct Page
        /// </summary>
        /// <returns>Admin AddProduct View</returns>
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <returns>Admins products View</returns> 
        /// <param name="product">Target product</param>
        [HttpPost]
        public IActionResult Add(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", product);
            }

            _productsService.Add(product);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Open Admin EditProduct Page
        /// </summary>
        /// <returns>Admin EditProduct View</returns>
        /// <param name="id">Target productId</param>
        public IActionResult Edit(Guid id)
        {
            var product = _productsService.Get(id);
            return View(product);
        }

        /// <summary>
        /// Update target product
        /// </summary>
        /// <returns>Admins products View</returns>
        [HttpPost]
        public IActionResult Update(Product product)
        {
            var isModelValid = _productsService.IsUpdateValid(ModelState, product);

            if (!isModelValid)
            {
                return View("Edit", product);
            }

            _productsService.Update(product);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete product by Id
        /// </summary>
        /// <returns>Admins products View</returns>
        /// <param name="id">Target productId</param>  
        public IActionResult Delete(Guid id)
        {
            _productsService.Delete(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get ViewComponent SpecificationsForm for product specifications depending on the category
        /// </summary>
        /// <returns>Relevant SpecificationsFormViewComponent</returns>
        /// <param name="category">Product category</param>   
        public IActionResult GetSpecificationsForm(ProductCategories category)
        {
            var emptySpecifications = new Dictionary<string, string>();
            var specificationsWithCategory = (emptySpecifications, category);
            return ViewComponent("SpecificationsForm", specificationsWithCategory);
        }

        /// <summary>
        /// Export all products info to excel
        /// </summary>
        /// <returns>Excel file with products info</returns>
        public IActionResult ExportToExcel()
        {
            var products = _productsService.GetAll();
            var excelStream = _excelService.ExportProducts(products);

            var downloadFileStream = new FileStreamResult(excelStream, Constants.ExcelFileContentType)
            {
                FileDownloadName = "Products.xlsx"
            };

            return downloadFileStream;
        }
    }
}
