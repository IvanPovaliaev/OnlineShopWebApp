using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class ComparisonsDbRepository : IComparisonsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ComparisonsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<ComparisonProduct>> GetAllAsync()
        {
            return await _databaseContext.ComparisonProducts.Include(c => c.Product)
                                                            .ThenInclude(p => p.Images)
                                                            .ToListAsync();
        }

        public async Task<ComparisonProduct> GetAsync(Guid id)
        {
            return await _databaseContext.ComparisonProducts.Include(c => c.Product)
                                                            .ThenInclude(p => p.Images)
                                                            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateAsync(ComparisonProduct product)
        {
            await _databaseContext.ComparisonProducts.AddAsync(product);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var comparison = await GetAsync(id);
            _databaseContext.ComparisonProducts.Remove(comparison);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAllAsync(string userId)
        {
            var comparisons = await _databaseContext.ComparisonProducts.Where(c => c.UserId == userId)
                                                                       .ToArrayAsync();
            _databaseContext.ComparisonProducts.RemoveRange(comparisons);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
