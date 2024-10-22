using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<Order> GetAll() => _orders;

        public Order Get(Guid id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            return order;
        }

        public void Create(Order order)
        {
            _orders.Add(order);
            _jsonRepositoryService.SaveChanges(FilePath, _orders);
        }

        public void UpdateStatus(Guid id, OrderStatus newStatus)
        {
            var repositoryOrder = Get(id);

            if (repositoryOrder is null)
            {
                return;
            }

            repositoryOrder.Status = newStatus;

            _jsonRepositoryService.SaveChanges(FilePath, _orders);
        }
    }
}
