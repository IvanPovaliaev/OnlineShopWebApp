using Newtonsoft.Json;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Collections.Generic;

namespace OnlineShopWebApp.Repositories
{
    public class InFileOrdersRepository : IOrdersRepository
    {
        public const string FilePath = @".\Data\Orders.json";
        private FileService _fileService;
        private List<Order> _orders;

        public InFileOrdersRepository(FileService fileService)
        {
            _fileService = fileService;
            Upload();
        }

        public void Create(Order order)
        {
            _orders.Add(order);
            SaveChanges();
        }

        /// <summary>
        /// Save changes in repository
        /// </summary>     
        private void SaveChanges()
        {
            var jsonData = JsonConvert.SerializeObject(_orders, Formatting.Indented);
            _fileService.Save(FilePath, jsonData);
        }

        /// <summary>
        /// Upload orders
        /// </summary>   
        private void Upload()
        {
            if (!_fileService.Exists(FilePath) || string.IsNullOrEmpty(_fileService.GetContent(FilePath)))
            {
                _orders = [];
                return;
            }

            var ordersJson = _fileService.GetContent(FilePath);

            _orders = JsonConvert.DeserializeObject<List<Order>>(ordersJson);
        }
    }
}
