using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        private readonly IMapper _mapper;
        private readonly ProductsService _productsService;
        private const string CookieCartKey = "Cart";

        public CookieCartsService(IHttpContextAccessor httpContextAccessor, IMapper mapper, ProductsService productsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _productsService = productsService;
        }

        /// <summary>
        /// Get cart from Cookie
        /// </summary>        
        /// <returns>CartViewModel from Cookie</returns>
        public virtual async Task<CartViewModel> GetViewModelAsync()
        {
            var cartJson = _httpContextAccessor.HttpContext?.Request.Cookies["Cart"];

            if (cartJson is null)
            {
                var cookieCart = new CookieCartViewModel();

                var cartVM = new CartViewModel()
                {
                    Id = cookieCart.Id,
                };

                return cartVM;
            }

            var cookieCart2 = JsonConvert.DeserializeObject<CookieCartViewModel>(cartJson);

            var cartVM2 = new CartViewModel()
            {
                Id = cookieCart2.Id
            };

            foreach (var position in cookieCart2.Positions)
            {
                var product = await _productsService.GetViewModelAsync(position.ProductId);
                var cartPosition = new CartPositionViewModel()
                {
                    Id = position.Id,
                    Product = product,
                    Quantity = position.Quantity,
                };

                cartVM2.Positions.Add(cartPosition);
            }

            return cartVM2;
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
        public virtual async Task DecreasePosition(Guid positionId)
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
        /// Delete cookie cart;
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

        ///// <summary>
        ///// Create a new cart for related user.
        ///// </summary>        
        ///// <param name="productId">product Id (GUID)</param>
        ///// <param name="userId">GUID user id</param>
        //private async Task CreateAsync(Guid productId, string userId)
        //{
        //    var cart = new Cart()
        //    {
        //        UserId = userId
        //    };

        //    await AddPositionAsync(cart, productId);
        //    SaveChanges(cart);
        //}

        /// <summary>
        /// Add new product position to cart.
        /// </summary>        
        /// <param name="cart">Cart with products</param>
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
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieCartKey, cartJson, options);
        }
    }
}
