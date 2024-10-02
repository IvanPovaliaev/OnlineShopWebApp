using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class InFileFavoritesRepository : IFavoritesRepository
    {
        public const string FilePath = @".\Data\FavoriteProducts.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<FavoriteProduct> _favorites;

        public InFileFavoritesRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _favorites = _jsonRepositoryService.Upload<FavoriteProduct>(FilePath);
        }

        public List<FavoriteProduct> GetAll() => _favorites;

        public FavoriteProduct Get(Guid favoriteProductId)
        {
            var favoriteProduct = _favorites.FirstOrDefault(f => f.Id == favoriteProductId);
            return favoriteProduct;
        }

        public void Create(FavoriteProduct product)
        {
            _favorites.Add(product);
            _jsonRepositoryService.SaveChanges(FilePath, _favorites);
        }

        public void Delete(Guid favoriteProductId)
        {
            var favoriteProduct = Get(favoriteProductId);
            _favorites.Remove(favoriteProduct);
            _jsonRepositoryService.SaveChanges(FilePath, _favorites);
        }

        public void DeleteAll(Guid userId)
        {
            _favorites.RemoveAll(f => f.UserId == userId);
            _jsonRepositoryService.SaveChanges(FilePath, _favorites);
        }

        public void DeleteAllByProductId(Guid productId)
        {
            _favorites.RemoveAll(f => f.Product.Id == productId);
            _jsonRepositoryService.SaveChanges(FilePath, _favorites);
        }
    }
}
