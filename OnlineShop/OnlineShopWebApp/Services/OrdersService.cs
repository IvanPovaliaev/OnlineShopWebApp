﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace OnlineShopWebApp.Services
{
    public class OrdersService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IExcelService _excelService;

        public OrdersService(IOrdersRepository ordersRepository, IExcelService excelService)
        {
            _ordersRepository = ordersRepository;
            _excelService = excelService;
        }

        /// <summary>
        /// Get all orders from repository
        /// </summary>
        /// <returns>List of all orders from repository</returns>
        public List<Order> GetAll() => _ordersRepository.GetAll();

        /// <summary>
        /// Create new order in repository
        /// </summary>
        /// <param name="order">Target creation model</param>
        public void Create(Order order)
        {
            _ordersRepository.Create(order);
        }

        /// <summary>
        /// Validates the order creation model
        /// </summary>        
        /// <returns>true if creation model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="order">Target creation model</param>
        public bool IsCreationValid(ModelStateDictionary modelState, Order order)
        {
            if (order.Positions.Count == 0)
            {
                modelState.AddModelError(string.Empty, "В заказе отсутствуют товары");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Update target order status in repository if possible
        /// </summary>
        /// <returns>Admin Orders View</returns>
        /// <param name="id">Order id (guid)</param>
        /// <param name="newStatus">New order status</param>
        public void UpdateStatus(Guid id, OrderStatus newStatus)
        {
            _ordersRepository.UpdateStatus(id, newStatus);
        }

        /// <summary>
        /// Get MemoryStream for all orders export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        public MemoryStream ExportAllToExcel()
        {
            var orders = GetAll();
            return _excelService.ExportOrders(orders);
        }
    }
}
