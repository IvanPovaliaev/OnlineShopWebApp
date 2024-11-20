using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Interfaces
{
    public interface IOrdersService
    {
        /// <summary>
        /// Get all orders from repository
        /// </summary>
        /// <returns>List of all OrderViewModel from repository</returns>
        Task<List<OrderViewModel>> GetAllAsync();

        /// <summary>
        /// Get all orders for related user
        /// </summary>
        /// <returns>List of all OrderViewModel from repository for related user</returns>
        Task<List<OrderViewModel>> GetAllAsync(string userId);

        /// <summary>
        /// Get last user order from repository 
        /// </summary>
        /// <returns>OrderViewModel; returns null if user doesn't have any orders</returns>
        Task<OrderViewModel> GetLastAsync(string userId);

        /// <summary>
        /// Create new order in repository
        /// </summary>
        /// <param name="userId">Target user ID</param>
        /// <param name="deliveryInfo">Related UserDeliveryInfoViewModel </param>
        /// <param name="positions">Target CartPosition List</param>
        Task CreateAsync(string userId, UserDeliveryInfoViewModel deliveryInfo, List<CartPosition> positions);

        /// <summary>
        /// Validates the order creation model
        /// </summary>        
        /// <returns>true if creation model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="positions">Target cart positions</param>
        bool IsCreationValid(ModelStateDictionary modelState, List<CartPosition> positions);

        /// <summary>
        /// Update target order status in repository if possible
        /// </summary>
        /// <returns>Admin Orders View</returns>
        /// <param name="id">Order id (guid)</param>
        /// <param name="newStatus">New order status</param>
        Task UpdateStatusAsync(Guid id, OrderStatusViewModel newStatus);

        /// <summary>
        /// Get MemoryStream for all orders export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        Task<MemoryStream> ExportAllToExcelAsync();
    }
}
