using Newtonsoft.Json;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class InFileCartsRepository : ICartsRepository
    {
        public const string FilePath = @".\Data\Carts.json";
        private FileService _fileService;
        private List<Cart> _carts;

        public InFileCartsRepository(FileService fileService)
        {
            _fileService = fileService;
            Upload();
        }

        public Cart Get(Guid userId)
        {
            return _carts.FirstOrDefault(cart => cart.UserId == userId);
        }

        public void Create(Cart cart)
        {
            _carts.Add(cart);
            SaveChanges();
        }

        public void Update(Cart cart)
        {
            var repositoryCart = _carts.FirstOrDefault(c => c.Id == cart.Id);

            if (repositoryCart is null)
            {
                return;
            }

            repositoryCart = cart;
            SaveChanges();
        }

        /// <summary>
        /// Save changes in repository
        /// </summary>     
        private void SaveChanges()
        {
            var jsonData = JsonConvert.SerializeObject(_carts, Formatting.Indented);
            _fileService.Save(FilePath, jsonData);
        }

        /// <summary>
        /// Upload carts
        /// </summary>   
        private void Upload()
        {
            var cartsJson = _fileService.GetContent(FilePath);

            if (cartsJson is null)
            {
                _carts = [];
                return;
            }

            _carts = JsonConvert.DeserializeObject<List<Cart>>(cartsJson);
        }
    }
}
