using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Db.Repositories
{
    public class OrdersDbRepository : IOrdersRepository
    {
        private readonly DatabaseContext _databaseContext;

        public OrdersDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<Order> GetAll()
        {
            return _databaseContext.Orders.Include(order => order.Info)
                                          .Include(order => order.Positions)
                                          .ThenInclude(position => position.Product)
                                          .ToList();
        }

        public Order Get(Guid id)
        {
            return _databaseContext.Orders.FirstOrDefault(o => o.Id == id)!;
        }

        public void Create(Order order)
        {
            _databaseContext.Orders.Add(order);
            _databaseContext.SaveChanges();
        }

        public void UpdateStatus(Guid id, OrderStatus newStatus)
        {
            var repositoryOrder = Get(id);

            if (repositoryOrder is null)
            {
                return;
            }

            repositoryOrder.Status = newStatus;

            _databaseContext.SaveChanges();
        }
    }
}
