using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Db.Repositories
{
    public class FavoritesDbRepository : IFavoritesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public FavoritesDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<FavoriteProduct> GetAll() => _databaseContext.FavoriteProducts.ToList();

        public FavoriteProduct Get(Guid id)
        {
            var favoriteProduct = _databaseContext.FavoriteProducts.FirstOrDefault(f => f.Id == id);
            return favoriteProduct!;
        }

        public void Create(FavoriteProduct product)
        {
            _databaseContext.FavoriteProducts.Add(product);
            _databaseContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var favoriteProduct = Get(id);
            _databaseContext.FavoriteProducts.Remove(favoriteProduct);
            _databaseContext.SaveChanges();
        }

        public void DeleteAll(Guid userId)
        {
            var favorites = _databaseContext.FavoriteProducts.Where(f => f.UserId == userId);
            _databaseContext.FavoriteProducts.RemoveRange(favorites);
            _databaseContext.SaveChanges();
        }

        public void DeleteAllByProductId(Guid productId)
        {
            var favorites = _databaseContext.FavoriteProducts.Where(f => f.Product.Id == productId);
            _databaseContext.FavoriteProducts.RemoveRange(favorites);
            _databaseContext.SaveChanges();
        }
    }
}
