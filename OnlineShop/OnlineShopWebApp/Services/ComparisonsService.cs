using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;

namespace OnlineShopWebApp.Services
{
    public class ComparisonsService
    {
        private readonly IOrdersRepository _ordersRepository;

        public ComparisonsService(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        /// <summary>
        /// Save Product to repository
        /// </summary>        
        public void Save(Order order)
        {
            _ordersRepository.Create(order);
        }

        /// <summary>
        /// Create a new ComparisonProduct for related user.
        /// </summary>        
        /// <param name="product">Position product</param>
        /// <param name="userId">GUID user id</param>
        private void Create(Guid userId)
        {

        }
    }
}
