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
            var cart = Get(userId);

            if (cart is null)
            {
                Create(product, userId);
                return;
            }

            var position = cart.Positions
                .FirstOrDefault(position => position.Product.Id == product.Id);

            if (position is null)
            {
                AddPosition(cart, product);
                _cartsRepository.Update(cart);
                return;
            }

            IncreasePosition(cart, position.Id);
        }

        /// <summary>
        /// Increase quantity of cart by Id position by 1
        /// </summary>        
        /// <param name="userId">UserId</param>
        /// <param name="positionId">Id of cart position</param>
        public void IncreasePosition(Guid userId, Guid positionId)
        {
            var cart = Get(userId);

            IncreasePosition(cart, positionId);
        }

        /// <summary>
        /// Increase quantity of target cart position by 1
        /// </summary>        
        /// <param name="cart">Target cart</param>
        /// <param name="positionId">Id of cart position</param>
        public void IncreasePosition(Cart cart, Guid positionId)
        {
            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);
            if (position is null)
            {
                return;
            }

            position.Quantity++;
            _cartsRepository.Update(cart);
        }

        /// <summary>
        /// Decrease quantity of cart by Id position by 1. If quantity should become 0, deletes this position.
        /// </summary>        
        /// <param name="userId">UserId</param>
        /// <param name="positionId">Id of cart position</param>
        public void DecreaseQuantity(Guid userId, Guid positionId)
        {
            var cart = Get(userId);

            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);
            if (position is null)
            {
                return;
            }

            if (position.Quantity == 1)
            {
                DeletePosition(cart, position);
                return;
            }

            position.Quantity--;
            _cartsRepository.Update(cart);
        }

        /// <summary>
        /// Delete target position in target cart. If positions count should become 0, deletes the cart.
        /// </summary>        
        /// <param name="cart">Tatget cart</param>
        /// <param name="position">Target position</param>
        private void DeletePosition(Cart cart, CartPosition position)
        {
            cart.Positions.Remove(position);

            if (cart.Positions.Count == 0)
            {
                _cartsRepository.Delete(cart);
            }
            _cartsRepository.Update(cart);
        }

        /// <summary>
        /// Delete cart of target user;
        /// </summary>        
        /// <param name="userId">Tatget userId</param>
        public void Delete(Guid userId)
        {
            var cart = Get(userId);
            _cartsRepository.Delete(cart);
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
