using Microsoft.EntityFrameworkCore;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Data.Repositories
{
    public class CartsDbRepository : ICartsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public CartsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Cart> GetAsync(string userId)
        {
            return await _databaseContext.Carts.Include(cart => cart.Positions)
                                               .ThenInclude(position => position.Product)
                                               .ThenInclude(p => p.Images)
                                               .FirstOrDefaultAsync(cart => cart.UserId == userId);
        }

        public async Task CreateAsync(Cart cart)
        {
            await _databaseContext.Carts.AddAsync(cart);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Cart cart)
        {
            var repositoryCart = await GetAsync(cart.UserId)!;

            if (repositoryCart is null)
            {
                return false;
            }

            foreach (var cartPosition in cart.Positions)
            {
                var existingPosition = repositoryCart.Positions.Find(p => p.Product.Id == cartPosition.Product.Id);

                if (existingPosition is null)
                {
                    repositoryCart.Positions.Add(cartPosition);
                    continue;
                }
                existingPosition.Quantity = cartPosition.Quantity;
            }

            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var repositoryCart = await GetAsync(userId);

            if (repositoryCart is null)
            {
                return false;
            }

            _databaseContext.Carts.Remove(repositoryCart);
            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}
