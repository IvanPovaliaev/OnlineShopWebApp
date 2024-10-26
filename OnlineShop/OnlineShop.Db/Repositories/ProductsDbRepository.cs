using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Db.Repositories
{
    public class ProductsDbRepository : IProductsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ProductsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<Product> GetAll() => _databaseContext.Products.ToList();

        public Product Get(Guid id)
        {
            var product = _databaseContext.Products.FirstOrDefault(p => p.Id == id);
            return product;
        }

        public void AddRange(List<Product> products)
        {
            _databaseContext.Products.AddRange(products);
            _databaseContext.SaveChanges();
        }

        public void Add(Product product)
        {
            _databaseContext.Products.Add(product);
            _databaseContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var product = Get(id);

            if (product is null)
            {
                return;
            }

            _databaseContext.Products.Remove(product);
            _databaseContext.SaveChanges();
        }

        public void Update(Product product)
        {
            var repositoryProduct = Get(product.Id);

            if (repositoryProduct is null)
            {
                return;
            }

            repositoryProduct.Name = product.Name;
            repositoryProduct.Cost = product.Cost;
            repositoryProduct.Description = product.Description;
            repositoryProduct.ImageUrl = product.ImageUrl;
            repositoryProduct.SpecificationsJson = product.SpecificationsJson;

            _databaseContext.SaveChanges();
        }
    }
}