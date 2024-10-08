using OnlineShopWebApp.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IOrdersRepository
    {
        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order">Target order</param>
        void Create(Order order);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        List<Order> GetAll();
    }
}
