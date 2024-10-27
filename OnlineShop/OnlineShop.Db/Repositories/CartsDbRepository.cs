using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Linq;

namespace OnlineShop.Db.Repositories
{
    public class CartsDbRepository : ICartsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public CartsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Cart Get(Guid userId)
        {
            return _databaseContext.Carts.Include(cart => cart.Positions)
                                         .ThenInclude(position => position.Product)
                                         .FirstOrDefault(cart => cart.UserId == userId)!;
        }

        public void Create(Cart cart)
        {
            _databaseContext.Carts.Add(cart);
            _databaseContext.SaveChanges();
        }

        public void Update(Cart cart)
        {
            var repositoryCart = Get(cart.UserId);

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

            _databaseContext.SaveChanges();
        }

        public void Delete(Cart cart)
        {
            var repositoryCart = Get(cart.UserId);

            if (repositoryCart is null)
            {
                return;
            }

            _databaseContext.Carts.Remove(repositoryCart);
            _databaseContext.SaveChanges();
        }
    }
}
