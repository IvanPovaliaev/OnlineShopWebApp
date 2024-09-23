using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Collections.Generic;

namespace OnlineShopWebApp.Repositories
{
    public class InFileOrdersRepository : IOrdersRepository
    {
        public const string FilePath = @".\Data\Orders.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<Order> _orders;

        public InFileOrdersRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _orders = _jsonRepositoryService.Upload<Order>(FilePath);
        }

        public void Create(Order order)
        {
            _orders.Add(order);
            _jsonRepositoryService.SaveChanges(FilePath, _orders);
        }
    }
}
