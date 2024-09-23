using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class CartsRepository
    {
        public const string FilePath = @".\Data\Carts.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<Cart> _carts;

        public CartsRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _carts = _jsonRepositoryService.Upload<Cart>(FilePath);
        }

        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>Cart for related user</returns>
        /// <param name="userId">GUID user id</param>
        public Cart Get(Guid userId)
        {
            return _carts.FirstOrDefault(cart => cart.UserId == userId);
        }

        /// <summary>
        /// Create a new cart
        /// </summary>    
        public void Create(Cart cart)
        {
            _carts.Add(cart);
            _jsonRepositoryService.SaveChanges(FilePath, _carts);
        }

        /// <summary>
        /// Update cart with identical id. If cart is not in the repository - does nothing.
        /// </summary>  
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
    }
}
