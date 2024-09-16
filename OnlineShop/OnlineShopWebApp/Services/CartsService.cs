using Newtonsoft.Json;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class CartsService
    {
        public const string FilePath = @".\Data\Carts.json";
        private List<Cart> _carts;

        public CartsService()
        {
            UploadCarts();
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
                return;
            }

            cartPosition.Quantity++;
            Save();
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
            _carts.Add(cart);
            Save();
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
            Save();
        }

        /// <summary>
        /// Save changes in storage
        /// </summary>     
        private void Save()
        {
            var jsonData = JsonConvert.SerializeObject(_carts, Formatting.Indented);
            FileService.Save(FilePath, jsonData);
        }

        /// <summary>
        /// Upload carts from storage
        /// </summary>   
        private void UploadCarts()
        {
            var cartsJson = FileService.GetContent(FilePath);

            if (cartsJson is null)
            {
                _carts = [];
                return;
            }

            _carts = JsonConvert.DeserializeObject<List<Cart>>(cartsJson);
        }
    }
}
