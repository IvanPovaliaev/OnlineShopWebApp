using AutoMapper;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class CartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IMapper _mapper;
        private readonly ProductsService _productsService;

        public CartsService(ICartsRepository cartsRepository, IMapper mapper, ProductsService productsService)
        {
            _cartsRepository = cartsRepository;
            _mapper = mapper;
            _productsService = productsService;
        }

        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>Cart for related user</returns>
        /// <param name="userId">GUID user id</param>
        public virtual async Task<Cart> GetAsync(string userId) => await _cartsRepository.GetAsync(userId);

        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>CartViewModel for related user</returns>
        /// <param name="userId">GUID user id</param>
        public virtual async Task<CartViewModel> GetViewModelAsync(string userId)
        {
            var cartDb = await GetAsync(userId);
            return _mapper.Map<CartViewModel>(cartDb);
        }

        /// <summary>
        /// Add product to users cart.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        public virtual async Task AddAsync(Guid productId, string userId)
        {
            var cart = await _cartsRepository.GetAsync(userId);

            if (cart is null)
            {
                await CreateAsync(productId, userId);
                return;
            }

            var position = cart.Positions.FirstOrDefault(position => position.Product.Id == productId);

            if (position is null)
            {
                await AddPositionAsync(cart, productId);
                await _cartsRepository.UpdateAsync(cart);
                return;
            }

            await IncreasePositionAsync(userId, position.Id);
        }

        /// <summary>
        /// Increase quantity of user cart position by 1
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        /// <param name="positionId">Id of cart position</param>
        public virtual async Task IncreasePositionAsync(string userId, Guid positionId)
        {
            var cart = await _cartsRepository.GetAsync(userId);

            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);

            if (position is null)
            {
                return;
            }

            position.Quantity++;

            await _cartsRepository.UpdateAsync(cart!);
        }

        /// <summary>
        /// Decrease position quantity in users cart by 1. If quantity should become 0, deletes this position.
        /// </summary>        
        /// <param name="userId">UserId</param>
        /// <param name="positionId">Id of cart position</param>
        public virtual async Task DecreasePosition(string userId, Guid positionId)
        {
            var cart = await _cartsRepository.GetAsync(userId);

            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);
            if (position is null)
            {
                return;
            }

            if (position.Quantity == 1)
            {
                await DeletePositionAsync(userId, positionId);
                return;
            }

            position.Quantity--;

            await _cartsRepository.UpdateAsync(cart!);
        }

        /// <summary>
        /// Delete cart of target user;
        /// </summary>        
        /// <param name="userId">Target userId</param>
        public virtual async Task DeleteAsync(string userId) => await _cartsRepository.DeleteAsync(userId);

        /// <summary>
        /// Delete target position in users cart. If positions count should become 0, deletes the cart.
        /// </summary>        
        /// <param name="userId">Target UserId</param>
        /// <param name="positionId">Target positionId</param>
        public virtual async Task DeletePositionAsync(string userId, Guid positionId)
        {
            var cart = await _cartsRepository.GetAsync(userId);
            var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);

            if (position is null)
            {
                return;
            }

            cart!.Positions.Remove(position);
            await _cartsRepository.UpdateAsync(cart);
        }

        /// <summary>
        /// Create a new cart for related user.
        /// </summary>        
        /// <param name="productId">product Id (GUID)</param>
        /// <param name="userId">GUID user id</param>
        private async Task CreateAsync(Guid productId, string userId)
        {
            var cart = new Cart()
            {
                UserId = userId
            };

            await AddPositionAsync(cart, productId);
            await _cartsRepository.CreateAsync(cart);
        }

        /// <summary>
        /// Add new product position to cart.
        /// </summary>        
        /// <param name="cart">Cart with products</param>
        /// <param name="productId">product Id (GUID)</param>
        private async Task AddPositionAsync(Cart cart, Guid productId)
        {
            var product = await _productsService.GetAsync(productId);

            var position = new CartPosition()
            {
                Product = product,
                Quantity = 1,
                Cart = cart
            };

            cart.Positions.Add(position);
        }
    }
}
