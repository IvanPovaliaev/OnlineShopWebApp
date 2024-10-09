﻿using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductsService _productsService;
        private readonly OrdersService _ordersService;

        public AdminController(ProductsService productsService, OrdersService ordersService)
        {
            _productsService = productsService;
            _ordersService = ordersService;
        }

        /// <summary>
        /// Open Admin Orders Page
        /// </summary>
        /// <returns>Admin Orders View</returns>
        public IActionResult Orders()
        {
            var orders = _ordersService.GetAll();
            return View(orders);
        }

        /// <summary>
        /// Update target order status if possible
        /// </summary>
        /// <returns>Admin Orders View</returns>
        /// <param name="orderId">Order id (guid)</param>
        /// <param name="status">New order status</param>
        public IActionResult UpdateOrderStatus(Guid orderId, OrderStatus status)
        {
            _ordersService.UpdateStatus(orderId, status);
            return RedirectToAction("Orders");
        }

        /// <summary>
        /// Open Admin Users Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        public IActionResult Users()
        {
            return View();
        }

        /// <summary>
        /// Open Admin Roles Page
        /// </summary>
        /// <returns>Admin Roles View</returns>
        public IActionResult Roles()
        {
            return View();
        }

        /// <summary>
        /// Open Admin Products Page
        /// </summary>
        /// <returns>Admin Products View</returns>
        public IActionResult Products()
        {
            var products = _productsService.GetAll();
            return View(products);
        }

        /// <summary>
        /// Open Admin AddProduct Page
        /// </summary>
        /// <returns>Admin AddProduct View</returns>
        public IActionResult AddProduct()
        {
            return View();
        }

        /// <summary>
        /// Open Admin EditProduct Page
        /// </summary>
        /// <returns>Admin EditProduct View</returns>
        /// <param name="orderId">Product id (guid)</param>
        public IActionResult EditProduct(Guid productId)
        {
            var product = _productsService.Get(productId);
            return View(product);
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
                return View("AddProduct", product);
            }

            _productsService.Add(product);
            return RedirectToAction("Products");
        }

        /// <summary>
        /// Delete product by Id
        /// </summary>
        /// <returns>Admins products View</returns>
        /// <param name="productId">Target productId</param>  
        public IActionResult Delete(Guid productId)
        {
            _productsService.Delete(productId);
            return RedirectToAction("Products");
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
                return View("EditProduct", product);
            }

            _productsService.Update(product);
            return RedirectToAction("Products");
        }

        /// <summary>
        /// Get Partial View for product specifications depending on the category
        /// </summary>
        /// <returns>Relevant Partial View</returns>
        /// <param name="category">Product category</param>   
        public IActionResult GetSpecificationsForm(ProductCategories category)
        {
            var emptySpecifications = new Dictionary<string, string>();
            var specificationsWithCategory = (emptySpecifications, category);
            return ViewComponent("SpecificationsForm", specificationsWithCategory);
        }
    }
}
