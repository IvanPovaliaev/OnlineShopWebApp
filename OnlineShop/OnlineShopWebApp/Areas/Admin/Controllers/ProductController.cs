using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Db;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class ProductController : Controller
    {
        private readonly IProductsService _productsService;

        public ProductController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Open Admin Products Page
        /// </summary>
        /// <returns>Admin Products View</returns>
        public async Task<IActionResult> Index()
        {
            var products = await _productsService.GetAllAsync();
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
        public async Task<IActionResult> Add(AddProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", product);
            }

            await _productsService.AddAsync(product);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Open Admin EditProduct Page
        /// </summary>
        /// <returns>Admin EditProduct View</returns>
        /// <param name="id">Target productId</param>
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productsService.GetEditProductAsync(id);
            return View(product);
        }

        /// <summary>
        /// Update target product
        /// </summary>
        /// <returns>Admins products View</returns>
        [HttpPost]
        public async Task<IActionResult> Update(EditProductViewModel product)
        {
            var isModelValid = await _productsService.IsUpdateValidAsync(ModelState, product);

            if (!isModelValid)
            {
                return View("Edit", product);
            }

            await _productsService.UpdateAsync(product);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete product by Id
        /// </summary>
        /// <returns>Admins products View</returns>
        /// <param name="id">Target productId</param>  
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productsService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get ViewComponent SpecificationsForm for product specifications depending on the category
        /// </summary>
        /// <returns>Relevant SpecificationsFormViewComponent</returns>
        /// <param name="category">Product category</param>   
        public IActionResult GetSpecificationsForm(ProductCategoriesViewModel category)
        {
            var emptySpecifications = new Dictionary<string, string>();
            var specificationsWithCategory = (emptySpecifications, category);
            return ViewComponent("SpecificationsForm", specificationsWithCategory);
        }

        /// <summary>
        /// Export all products info to excel
        /// </summary>
        /// <returns>Excel file with products info</returns>
        public async Task<IActionResult> ExportToExcel()
        {
            var stream = await _productsService.ExportAllToExcelAsync();

            var downloadFileStream = new FileStreamResult(stream, Formats.ExcelFileContentType)
            {
                FileDownloadName = "Products.xlsx"
            };

            return downloadFileStream;
        }
    }
}
