using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class CartsService
    {
        private readonly ICartsRepository _cartsRepository;

        public CartsService(ICartsRepository cartsRepository)
        {
            _cartsRepository = cartsRepository;
        }

        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>Cart for related user</returns>
        /// <param name="userId">GUID user id</param>
        public Cart Get(Guid userId)
        {
            return _cartsRepository.Get(userId);
        }

        /// <summary>
        /// Add product to users cart.
        /// </summary>        
        /// <param name="product">Position product</param>
        /// <param name="userId">GUID user id</param>
        public void Add(Product product, Guid userId)
        {
            var userCart = Get(userId);

            if (userCart is null)
            {
                Create(product, userId);
                return;
            }

            var cartPosition = userCart.Positions
                .FirstOrDefault(position => position.Product.Id == product.Id);

            if (cartPosition is null)
            {
                AddPosition(userCart, product);
                _cartsRepository.Update(userCart);
                return;
            }

            cartPosition.Quantity++;
            _cartsRepository.Update(userCart);
        }

        /// <summary>
        /// Create a new cart for related user.
        /// </summary>        
        /// <param name="product">Position product</param>
        /// <param name="userId">GUID user id</param>
        private void Create(Product product, Guid userId)
        {
            var cart = new Cart(userId);
            AddPosition(cart, product);
            _cartsRepository.Create(cart);
        }

        /// <summary>
        /// Add new product position to cart.
        /// </summary>        
        /// <param name="cart">Cart with products</param>
        /// <param name="product">Position product</param>
        private void AddPosition(Cart cart, Product product)
        {
            var newPosition = new CartPosition(product, 1);
            cart.Positions.Add(newPosition);
        }
    }
}
