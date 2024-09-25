using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Services
{
    public class OrdersService
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrdersService(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        /// <summary>
        /// Save order to repository
        /// </summary>        
        public void Save(Order order)
        {
            _ordersRepository.Create(order);
        }
    }
}
