﻿using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Interfaces
{
    public interface IOrdersRepository
    {
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        Task<List<Order>> GetAllAsync();

        /// <summary>
        /// Get order by GUID
        /// </summary>
        /// <returns>Order; returns null if order not found</returns>
        /// <param name="id">Orders id GUID</param>
        Task<Order> GetAsync(Guid id);

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order">Target order</param>
        Task CreateAsync(Order order);

        /// <summary>
        /// Update order status with identical id. If order is not in the repository - does nothing.
        /// </summary>
        /// <param name="id">Orders id GUID</param>
        /// <param name="newStatus">new OrderStatus</param>
        Task UpdateStatusAsync(Guid id, OrderStatus newStatus);
    }
}