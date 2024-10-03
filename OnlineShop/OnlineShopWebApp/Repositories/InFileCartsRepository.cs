using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class InFileCartsRepository : ICartsRepository
    {
        public const string FilePath = @".\Data\Carts.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<Cart> _carts;

        public InFileCartsRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _carts = _jsonRepositoryService.Upload<Cart>(FilePath);
        }

        public Cart Get(Guid userId)
        {
            return _carts.FirstOrDefault(cart => cart.UserId == userId);
        }

        public void Create(Cart cart)
        {
            _carts.Add(cart);
            _jsonRepositoryService.SaveChanges(FilePath, _carts);
        }

        public void Update(Cart cart)
        {
            var repositoryCart = _carts.FirstOrDefault(c => c.Id == cart.Id);

            if (repositoryCart is null)
            {
                return;
            }

            repositoryCart = cart;
            _jsonRepositoryService.SaveChanges(FilePath, _carts);
        }

        public void Delete(Cart cart)
        {
            var repositoryCart = _carts.FirstOrDefault(c => c.Id == cart.Id);

            if (repositoryCart is null)
            {
                return;
            }

            _carts.Remove(repositoryCart);
            _jsonRepositoryService.SaveChanges(FilePath, _carts);
        }
    }
}
