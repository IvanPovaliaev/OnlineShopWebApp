using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IOrdersRepository
    {
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        List<Order> GetAll();

        /// <summary>
        /// Get order by GUID
        /// </summary>
        /// <returns>Order; returns null if order not found</returns>
        /// <param name="id">Orders id GUID</param>
        Order Get(Guid id);

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order">Target order</param>
        void Create(Order order);

        /// <summary>
        /// Update order status with identical id. If order is not in the repository - does nothing.
        /// </summary>
        /// <param name="id">Orders id GUID</param>
        /// <param name="newStatus">new OrderStatus</param>
        void UpdateStatus(Guid id, OrderStatus newStatus);
    }
}
