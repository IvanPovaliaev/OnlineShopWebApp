using LinqSpecs;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Data.Repositories
{
    public class OrdersDbRepository : IOrdersRepository
    {
        private readonly DatabaseContext _databaseContext;

        public OrdersDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<Order>> GetAllAsync(Specification<Order>? specification = null)
        {
            var query = _databaseContext.Orders.AsQueryable();

            if (specification is not null)
            {
                query = query.Where(specification.ToExpression());
            }

            return await query.Include(order => order.Info)
                              .Include(order => order.Positions)
                              .ThenInclude(position => position.Product)
                              .ToListAsync();
        }

        public async Task<Order?> GetAsync(Guid id)
        {
            return await _databaseContext.Orders.FindAsync(id);
        }

        public async Task<Guid?> CreateAsync(Order order)
        {
            await _databaseContext.Orders.AddAsync(order);
            var result = await _databaseContext.SaveChangesAsync();
            return result > 0 ? order.Id : null;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, OrderStatus newStatus)
        {
            var repositoryOrder = await GetAsync(id);

            if (repositoryOrder is null)
            {
                return false;
            }

            repositoryOrder.Status = newStatus;

            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}
