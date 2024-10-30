using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class OrdersDbRepository : IOrdersRepository
    {
        private readonly DatabaseContext _databaseContext;

        public OrdersDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _databaseContext.Orders.Include(order => order.Info)
                                          .Include(order => order.Positions)
                                          .ThenInclude(position => position.Product)
                                          .ToListAsync();
        }

        public async Task<Order> GetAsync(Guid id)
        {
            return await _databaseContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task CreateAsync(Order order)
        {
            await _databaseContext.Orders.AddAsync(order);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Guid id, OrderStatus newStatus)
        {
            var repositoryOrder = await GetAsync(id);

            if (repositoryOrder is null)
            {
                return;
            }

            repositoryOrder.Status = newStatus;

            await _databaseContext.SaveChangesAsync();
        }
    }
}
