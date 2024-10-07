using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        /// <summary>
        /// Validates the order creation model
        /// </summary>        
        /// <returns>true if creation model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// /// <param name="register">Target creation model</param>
        public bool IsCreationValid(ModelStateDictionary modelState, Order order)
        {
            if (order.Positions.Count == 0)
            {
                modelState.AddModelError(string.Empty, "В заказе отсутствуют товары");
                return false;
            }

            return true;
        }
    }
}
