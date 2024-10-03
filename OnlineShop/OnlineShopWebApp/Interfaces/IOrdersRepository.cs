using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Interfaces
{
    public interface IOrdersRepository
    {
        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order">Target order</param>
        void Create(Order order);
    }
}
