using AutoMapper;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Models;
using System;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class CartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;
        private readonly ProductsService _productsService;

        public CartsService(ICartsRepository cartsRepository, IProductsRepository productsRepository, IMapper mapper, ProductsService productsService)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _mapper = mapper;
            _productsService = productsService;
        }

        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>CartViewModel for related user</returns>
        /// <param name="userId">GUID user id</param>
        public CartViewModel Get(Guid userId)
        {
            var cartDb = _cartsRepository.Get(userId);
            return _mapper.Map<CartViewModel>(cartDb);
        }

        /// <summary>
        /// Add product to users cart.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        public void Add(Guid productId, Guid userId)
        {
            var cart = _cartsRepository.Get(userId);

            if (cart is null)
            {
                Create(productId, userId);
                return;
            }

            var position = cart.Positions.FirstOrDefault(position => position.Product.Id == productId);

            if (position is null)
            {
                AddPosition(cart, productId);

                _cartsRepository.Update(cart);
                return;
            }

            IncreasePosition(userId, position.Id);
        }

        /// <summary>
        /// Increase quantity of user cart position by 1
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        /// <param name="positionId">Id of cart position</param>
        public void IncreasePosition(Guid userId, Guid positionId)
        {
            var cart = _cartsRepository.Get(userId);

            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);

            if (position is null)
            {
                return;
            }

            position.Quantity++;

            _cartsRepository.Update(cart!);
        }

        /// <summary>
        /// Decrease position quantity in users cart by 1. If quantity should become 0, deletes this position.
        /// </summary>        
        /// <param name="userId">UserId</param>
        /// <param name="positionId">Id of cart position</param>
        public void DecreasePosition(Guid userId, Guid positionId)
        {
            var cart = _cartsRepository.Get(userId);

            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);
            if (position is null)
            {
                return;
            }

            if (position.Quantity == 1)
            {
                DeletePosition(userId, positionId);
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
            var cart = _cartsRepository.Get(userId);

            _cartsRepository.Delete(cart);
        }

        /// <summary>
        /// Delete target position in users cart. If positions count should become 0, deletes the cart.
        /// </summary>        
        /// <param name="userId">Target UserId</param>
        /// <param name="positionId">Target positionId</param>
        public void DeletePosition(Guid userId, Guid positionId)
        {
            var cart = _cartsRepository.Get(userId);
            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);

            if (position is null)
            {
                return;
            }

            cart!.Positions.Remove(position);
            _cartsRepository.Update(cart);
        }

        /// <summary>
        /// Create a new cart for related user.
        /// </summary>        
        /// <param name="productId">product Id (GUID)</param>
        /// <param name="userId">GUID user id</param>
        private void Create(Guid productId, Guid userId)
        {
            var cart = new Cart(userId);

            AddPosition(cart, productId);
            _cartsRepository.Create(cart);
        }

        /// <summary>
        /// Add new product position to cart.
        /// </summary>        
        /// <param name="cart">Cart with products</param>
        /// <param name="productId">product Id (GUID)</param>
        private void AddPosition(Cart cart, Guid productId)
        {
            var product = _productsRepository.Get(productId);

            var position = new CartPosition(product, cart);
            cart.Positions.Add(position);
        }
    }
}
