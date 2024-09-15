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
            UpdatesCarts();
        }

        public Cart Get(Guid userId)
        {
            return _carts.FirstOrDefault(cart => cart.UserId == userId);
        }

        public void Add(Product product, Guid userId)
        {
            var userCart = Get(userId);

            if (userCart is null)
            {
                CreateNewCart(product, userId);
                return;
            }

            var cartPosition = userCart.Positions
                .FirstOrDefault(position => position.Product.Id == product.Id);

            if (cartPosition is null)
            {
                AddNewPosition(userCart, product);
                return;
            }

            cartPosition.Quantity++;
            Save();
        }

        private void CreateNewCart(Product product, Guid userId)
        {
            var cart = new Cart(userId);
            AddNewPosition(cart, product);
            _carts.Add(cart);
            Save();
        }

        private void AddNewPosition(Cart cart, Product product)
        {
            var newPosition = new CartPosition(product, 1);
            cart.Positions.Add(newPosition);
            Save();
        }

        private void Save()
        {
            var jsonData = JsonConvert.SerializeObject(_carts, Formatting.Indented);
            FileService.Save(FilePath, jsonData);
        }

        private void UpdatesCarts()
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
