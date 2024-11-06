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
using System.Threading.Tasks;

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
        public virtual async Task<List<OrderViewModel>> GetAllAsync()
        {
            var orders = await _ordersRepository.GetAllAsync();
            return orders.Select(_mapper.Map<OrderViewModel>)
                         .ToList();
        }

        /// <summary>
        /// Get last user order from repository 
        /// </summary>
        /// <returns>OrderViewModel; returns null if user doesn't have any orders</returns>
        public virtual async Task<OrderViewModel> GetLastAsync(Guid userId)
        {
            var orders = await GetAllAsync();
            return orders.LastOrDefault(o => o.UserId == userId)!;
        }

        /// <summary>
        /// Create new order in repository
        /// </summary>
        /// <param name="userId">Target user ID</param>
        /// <param name="deliveryInfo">Related UserDeliveryInfoViewModel </param>
        /// <param name="positions">Target CartPosition List</param>
        public virtual async Task CreateAsync(Guid userId, UserDeliveryInfoViewModel deliveryInfo, List<CartPosition> positions)
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

            await _ordersRepository.CreateAsync(order);
        }

        /// <summary>
        /// Validates the order creation model
        /// </summary>        
        /// <returns>true if creation model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="positions">Target cart positions</param>
        public virtual bool IsCreationValid(ModelStateDictionary modelState, List<CartPosition> positions)
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
        public virtual async Task UpdateStatusAsync(Guid id, OrderStatusViewModel newStatus)
        {
            await _ordersRepository.UpdateStatusAsync(id, (OrderStatus)newStatus);
        }

        /// <summary>
        /// Get MemoryStream for all orders export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        public virtual async Task<MemoryStream> ExportAllToExcelAsync()
        {
            var orders = await GetAllAsync();
            return _excelService.ExportOrders(orders);
        }
    }
}
