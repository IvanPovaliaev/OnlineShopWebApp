using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class ProductsDbRepository : IProductsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ProductsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<Product>> GetAllAsync() => await _databaseContext.Products.ToListAsync();

        public async Task<Product> GetAsync(Guid id)
        {
            return await _databaseContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddRangeAsync(List<Product> products)
        {
            await _databaseContext.Products.AddRangeAsync(products);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _databaseContext.Products.AddAsync(product);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await GetAsync(id);

            if (product is null)
            {
                return;
            }

            _databaseContext.Products.Remove(product);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            var repositoryProduct = await GetAsync(product.Id);

            if (repositoryProduct is null)
            {
                return;
            }

            repositoryProduct.Name = product.Name;
            repositoryProduct.Cost = product.Cost;
            repositoryProduct.Description = product.Description;
            repositoryProduct.ImageUrl = product.ImageUrl;
            repositoryProduct.SpecificationsJson = product.SpecificationsJson;

            await _databaseContext.SaveChangesAsync();
        }
    }
}