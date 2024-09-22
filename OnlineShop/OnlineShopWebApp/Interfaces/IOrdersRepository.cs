using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Interfaces
{
    public interface IOrdersRepository
    {
        /// <summary>
        /// Create a new order
        /// </summary>    
        void Create(Order order);
    }
}
