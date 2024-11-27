using AutoMapper;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IMapper _mapper;
        private readonly IProductsService _productsService;

        public CartsService(ICartsRepository cartsRepository, IMapper mapper, IProductsService productsService)
        {
            _cartsRepository = cartsRepository;
            _mapper = mapper;
            _productsService = productsService;
        }

        public async Task<Cart> GetAsync(string userId) => await _cartsRepository.GetAsync(userId);

        public async Task<CartViewModel> GetViewModelAsync(string userId)
        {
            var cartDb = await GetAsync(userId);
            return _mapper.Map<CartViewModel>(cartDb);
        }

        public async Task AddAsync(Guid productId, string userId)
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

        public async Task IncreasePositionAsync(string userId, Guid positionId)
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

        public async Task DecreasePositionAsync(string userId, Guid positionId)
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

        public async Task DeleteAsync(string userId) => await _cartsRepository.DeleteAsync(userId);

        public async Task DeletePositionAsync(string userId, Guid positionId)
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

        public async Task ReplaceFromCookieAsync(CartViewModel cookieCart, string userId)
        {
            if (cookieCart is null || cookieCart.Positions.Count == 0)
            {
                return;
            }

            var cart = await _cartsRepository.GetAsync(userId);

            cart ??= new Cart()
            {
                UserId = userId
            };

            cart.Positions.Clear();

            foreach (var position in cookieCart.Positions)
            {
                var productDb = await _productsService.GetAsync(position.Product.Id);
                var newCartPosition = new CartPosition()
                {
                    Product = productDb,
                    Quantity = position.Quantity,
                    Cart = cart
                };

                cart.Positions.Add(newCartPosition);
            }

            if (cart.Id == new Guid())
            {
                await _cartsRepository.CreateAsync(cart);
                return;
            }

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
