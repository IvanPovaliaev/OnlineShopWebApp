using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class CartsDbRepository : ICartsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public CartsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Cart> GetAsync(Guid userId)
        {
            return await _databaseContext.Carts.Include(cart => cart.Positions)
                                               .ThenInclude(position => position.Product)
                                               .FirstOrDefaultAsync(cart => cart.UserId == userId);
        }

        public async Task CreateAsync(Cart cart)
        {
            await _databaseContext.Carts.AddAsync(cart);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            var repositoryCart = await GetAsync(cart.UserId)!;

            if (repositoryCart is null)
            {
                return;
            }

            foreach (var cartPosition in cart.Positions)
            {
                var existingPosition = repositoryCart.Positions.FirstOrDefault(p => p.Product.Id == cartPosition.Product.Id);

                if (existingPosition is null)
                {
                    repositoryCart.Positions.Add(cartPosition);
                    continue;
                }
                existingPosition.Quantity = cartPosition.Quantity;
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid userId)
        {
            var repositoryCart = await GetAsync(userId);

            if (repositoryCart is null)
            {
                return;
            }

            _databaseContext.Carts.Remove(repositoryCart);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
