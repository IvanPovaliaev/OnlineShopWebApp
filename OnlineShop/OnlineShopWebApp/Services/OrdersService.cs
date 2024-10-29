using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class OrdersService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;

        public OrdersService(IOrdersRepository ordersRepository, IExcelService excelService, IMapper mapper)
        {
            _ordersRepository = ordersRepository;
            _excelService = excelService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all orders from repository
        /// </summary>
        /// <returns>List of all OrderViewModel from repository</returns>
        public List<OrderViewModel> GetAll()
        {
            return _ordersRepository.GetAll()
                                    .Select(_mapper.Map<OrderViewModel>)
                                    .ToList();
        }

        /// <summary>
        /// Get last user order grom repository 
        /// </summary>
        /// <returns>OrderViewModel; returns null if user doesn't have any orders</returns>
        public OrderViewModel GetLast(Guid userId)
        {
            return GetAll().LastOrDefault(o => o.UserId == userId)!;
        }

        /// <summary>
        /// Create new order in repository
        /// </summary>
        /// <param name="userId">Target user ID</param>
        /// <param name="deliveryInfo">Related UserDeliveryInfoViewModel </param>
        /// <param name="positions">Target CartPosition List</param>
        public void Create(Guid userId, UserDeliveryInfoViewModel deliveryInfo, List<CartPosition> positions)
        {
            var deliveryInfoDb = _mapper.Map<UserDeliveryInfo>(deliveryInfo);
            var order = new Order
            {
                UserId = userId,
                Info = deliveryInfoDb
            };

            order.Positions = positions.Select(p => new OrderPosition()
            {
                Product = p.Product,
                Quantity = p.Quantity,
                Order = order
            }).ToList();

            _ordersRepository.Create(order);
        }

        /// <summary>
        /// Validates the order creation model
        /// </summary>        
        /// <returns>true if creation model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="positions">Target cart positions</param>
        public bool IsCreationValid(ModelStateDictionary modelState, List<CartPosition> positions)
        {
            if (positions.Count == 0)
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
        public void UpdateStatus(Guid id, OrderStatusViewModel newStatus)
        {
            _ordersRepository.UpdateStatus(id, (OrderStatus)newStatus);
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
