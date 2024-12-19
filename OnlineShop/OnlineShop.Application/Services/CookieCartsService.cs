using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
    public class CookieCartsService : ICookieCartsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IProductsService _productsService;
        private readonly CookieCartOptions _cookieCartOptions;

        public CookieCartsService(IHttpContextAccessor httpContextAccessor, IOptionsSnapshot<CookieCartOptions> cookieCartOptions, IProductsService productsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cookieCartOptions = cookieCartOptions.Value;
            _productsService = productsService;
        }

        public async Task<CartViewModel> GetViewModelAsync()
        {
            var cartJson = _httpContextAccessor.HttpContext?.Items[_cookieCartOptions.CartKey]?.ToString();
            cartJson ??= _httpContextAccessor.HttpContext?.Request.Cookies[_cookieCartOptions.CartKey] ?? string.Empty;

            var cookieCart = JsonConvert.DeserializeObject<CookieCartViewModel>(cartJson);
            cookieCart ??= new CookieCartViewModel();

            var cartVM = new CartViewModel()
            {
                Id = cookieCart.Id
            };

            foreach (var position in cookieCart.Positions)
            {
                var product = await _productsService.GetViewModelAsync(position.ProductId);

                var cartPosition = new CartPositionViewModel()
                {
                    Id = position.Id,
                    Product = product,
                    Quantity = position.Quantity,
                };

                cartVM.Positions.Add(cartPosition);
            }

            return cartVM;
        }

        public async Task AddAsync(Guid productId)
        {
            var cart = await GetViewModelAsync();

            var position = cart.Positions.FirstOrDefault(position => position.Product?.Id == productId);

            if (position is null)
            {
                await AddPositionAsync(cart, productId);
                SaveChanges(cart);
                return;
            }

            await IncreasePositionAsync(position.Id);
        }

        public async Task IncreasePositionAsync(Guid positionId)
        {
            var cart = await GetViewModelAsync();

            var position = cart.Positions.FirstOrDefault(pos => pos.Id == positionId);

            if (position is null)
            {
                return;
            }

            position.Quantity++;

            SaveChanges(cart);
        }

        public async Task DecreasePositionAsync(Guid positionId)
        {
            var cart = await GetViewModelAsync();

            var position = cart.Positions.FirstOrDefault(pos => pos.Id == positionId);
            if (position is null)
            {
                return;
            }

            if (position.Quantity == 1)
            {
                await DeletePositionAsync(positionId);
                return;
            }

            position.Quantity--;

            SaveChanges(cart);
        }

        public void Delete()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(_cookieCartOptions.CartKey);
        }

        public async Task DeletePositionAsync(Guid positionId)
        {
            var cart = await GetViewModelAsync();
            var position = cart.Positions.FirstOrDefault(pos => pos.Id == positionId);

            if (position is null)
            {
                return;
            }

            cart!.Positions.Remove(position);
            SaveChanges(cart);
        }

        /// <summary>
        /// Add new product position to cookie cart.
        /// </summary>        
        /// <param name="cart">Cookie cart</param>
        /// <param name="productId">product Id (GUID)</param>
        private async Task AddPositionAsync(CartViewModel cart, Guid productId)
        {
            var product = await _productsService.GetViewModelAsync(productId);

            var position = new CartPositionViewModel()
            {
                Id = Guid.NewGuid(),
                Product = product,
                Quantity = 1
            };

            cart.Positions.Add(position);
        }

        /// <summary>
        /// Save target cart to cookie
        /// </summary>
        /// <param name="cart">CartViewModel</param>
        private void SaveChanges(CartViewModel cart)
        {
            var cookieCart = new CookieCartViewModel()
            {
                Id = cart.Id
            };

            foreach (var position in cart.Positions)
            {
                var cookiePosition = new CookieCartPositionViewModel
                {
                    Id = position.Id,
                    ProductId = position.Product.Id,
                    Quantity = position.Quantity
                };

                cookieCart.Positions.Add(cookiePosition);
            }

            var cartJson = JsonConvert.SerializeObject(cookieCart);
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(_cookieCartOptions.ExpiresDays)
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(_cookieCartOptions.CartKey, cartJson, options);
            _httpContextAccessor.HttpContext?.Items.Add(_cookieCartOptions.CartKey, cartJson);
        }
    }
}
