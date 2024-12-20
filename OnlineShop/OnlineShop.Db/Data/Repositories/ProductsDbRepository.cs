﻿using LinqSpecs;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Data.Repositories
{
    public class ProductsDbRepository : IProductsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ProductsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<Product>> GetAllAsync(Specification<Product>? specification = null)
        {
            var query = _databaseContext.Products.AsQueryable();

            if (specification is not null)
            {
                query = query.Where(specification);
            }

            return await query.Include(p => p.Images)
                              .ToListAsync();
        }

        public async Task<Product?> GetAsync(Guid id)
        {
            return await _databaseContext.Products
                                         .Include(p => p.Images)
                                         .FirstOrDefaultAsync(p => p.Id == id);
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _databaseContext.Products.FindAsync(id);

            if (product is null)
            {
                return false;
            }

            _databaseContext.Products.Remove(product);
            await _databaseContext.SaveChangesAsync();
            return true;
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

            if (product.Images.Count > 0)
            {
                repositoryProduct.Images = product.Images;
            }

            repositoryProduct.SpecificationsJson = product.SpecificationsJson;

            await _databaseContext.SaveChangesAsync();
        }
    }
}