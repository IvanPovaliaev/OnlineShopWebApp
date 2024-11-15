using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineShopWebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class CookieCartsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ProductsService _productsService;
        private readonly string CookieCartKey;
        private readonly int ExpiresTime;

        public CookieCartsService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ProductsService productsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _productsService = productsService;

            CookieCartKey = _configuration["CookiesSettings:CartKey"]!;
            ExpiresTime = Convert.ToInt32(_configuration["CookiesSettings:ExpiresTime"]);
        }

        /// <summary>
        /// Get cart from Cookie
        /// </summary>        
        /// <returns>CartViewModel from Cookie</returns>
        public virtual async Task<CartViewModel> GetViewModelAsync()
        {
            var cartJson = _httpContextAccessor.HttpContext?.Items[CookieCartKey]?.ToString();
            cartJson ??= _httpContextAccessor.HttpContext?.Request.Cookies[CookieCartKey] ?? string.Empty;

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

        /// <summary>
        /// Add product to users cart.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        public virtual async Task AddAsync(Guid productId)
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

        /// <summary>
        /// Increase quantity of cookie cart position by 1
        /// </summary>        
        /// <param name="positionId">Id of cart position</param>
        public virtual async Task IncreasePositionAsync(Guid positionId)
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

        /// <summary>
        /// Decrease position quantity in cookie cart by 1. If quantity should become 0, deletes this position.
        /// </summary>        
        /// <param name="positionId">Id of cart position</param>
        public virtual async Task DecreasePositionAsync(Guid positionId)
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

        /// <summary>
        /// Delete cookie cart
        /// </summary>
        public virtual void Delete()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieCartKey);
        }

        /// <summary>
        /// Delete target position in cookie users
        /// </summary>
        /// <param name="positionId">Target positionId</param>
        public virtual async Task DeletePositionAsync(Guid positionId)
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
                Expires = DateTimeOffset.UtcNow.AddDays(ExpiresTime)
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieCartKey, cartJson, options);
            _httpContextAccessor.HttpContext?.Items.Add(CookieCartKey, cartJson);
        }
    }
}
