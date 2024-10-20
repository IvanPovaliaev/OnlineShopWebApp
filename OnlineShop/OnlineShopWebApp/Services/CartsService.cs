using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class CartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly ProductsService _productsService;

        public CartsService(ICartsRepository cartsRepository, ProductsService productsService)
        {
            _cartsRepository = cartsRepository;
            _productsService = productsService;
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
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        public void Add(Guid productId, Guid userId)
        {
            var product = _productsService.Get(productId);
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
            _cartsRepository.Update(cart!);
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
                DeletePosition(cart!, position);
                return;
            }

            position.Quantity--;
            _cartsRepository.Update(cart!);
        }

        /// <summary>
        /// Delete all CartPositions related to product Id.
        /// </summary>
        /// <param name="productId">Target product Id (guid)</param>
        public void DeletePositionsByProductId(Guid productId)
        {
            _cartsRepository.DeletePositionsByProductId(productId);
        }

        /// <summary>
        /// Delete cart of target user;
        /// </summary>        
        /// <param name="userId">Target userId</param>
        public void Delete(Guid userId)
        {
            var cart = Get(userId);
            _cartsRepository.Delete(cart);
        }

        /// <summary>
        /// Delete target position in users cart. If positions count should become 0, deletes the cart.
        /// </summary>        
        /// <param name="userId">Target UserId</param>
        /// <param name="positionId">Target positionId</param>
        public void DeletePosition(Guid userId, Guid positionId)
        {
            var cart = Get(userId);
            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);

            if (position is null)
            {
                return;
            }

            DeletePosition(cart!, position);
        }

        /// <summary>
        /// Delete target position in target cart. If positions count should become 0, deletes the cart.
        /// </summary>        
        /// <param name="cart">Target cart</param>
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
