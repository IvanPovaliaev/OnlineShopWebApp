using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class FavoritesDbRepository : IFavoritesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public FavoritesDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<FavoriteProduct>> GetAllAsync()
        {
            return await _databaseContext.FavoriteProducts.Include(f => f.Product)
                                                   .ToListAsync();
        }

        public async Task<FavoriteProduct> GetAsync(Guid id)
        {
            return await _databaseContext.FavoriteProducts.Include(f => f.Product)
                                                          .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task CreateAsync(FavoriteProduct product)
        {
            await _databaseContext.FavoriteProducts.AddAsync(product)!;
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var favoriteProduct = await GetAsync(id);
            _databaseContext.FavoriteProducts.Remove(favoriteProduct);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAllAsync(Guid userId)
        {
            var favorites = await _databaseContext.FavoriteProducts.Where(f => f.UserId == userId)
                                                                   .ToArrayAsync();
            _databaseContext.FavoriteProducts.RemoveRange(favorites);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
