using AutoMapper;
using Bogus;
using LinqSpecs;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Application.Services;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Application.Services
{
    public class FavoritesServiceTests
    {
        private readonly Mock<IFavoritesRepository> _favoritesRepositoryMock;
        private readonly Mock<IProductsService> _productsServiceMock;
        private readonly FavoritesService _favoritesService;
        private readonly IMapper _mapper;

        private readonly string _userId;
        private readonly List<FavoriteProduct> _fakeFavoriteProducts;
        private readonly Faker<Product> _productFaker;

        public FavoritesServiceTests(Mock<IFavoritesRepository> favoritesRepositoryMock, Mock<IProductsService> productsServiceMock, IMapper mapper, FakerProvider fakerProvider)
        {
            _favoritesRepositoryMock = favoritesRepositoryMock;
            _productsServiceMock = productsServiceMock;

            _mapper = mapper;

            _favoritesService = new FavoritesService(
                _favoritesRepositoryMock.Object,
                _mapper,
                _productsServiceMock.Object);

            _userId = fakerProvider.UserId;
            _productFaker = fakerProvider.ProductFaker;
            _fakeFavoriteProducts = fakerProvider.FakeFavoriteProducts;
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasFavorites_ReturnsUserFavorites()
        {
            // Arrange
            var expectedCount = _fakeFavoriteProducts.Count;
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<FavoriteProduct>>()))
                                    .ReturnsAsync(_fakeFavoriteProducts);

            // Act
            var result = await _favoritesService.GetAllAsync(_userId);

            // Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, favorite => Assert.IsType<FavoriteProductViewModel>(favorite));
            Assert.All(result, favorite => Assert.Equal(_userId, favorite.UserId));
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasNoFavorites_ReturnsEmpty()
        {
            // Arrange
            var anotherUserId = Guid.NewGuid().ToString();
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<FavoriteProduct>>()))
                                    .ReturnsAsync([]);

            // Act
            var result = await _favoritesService.GetAllAsync(anotherUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_WhenProductIsNotInFavorites_AddProductToFavorites()
        {
            // Arrange
            var fakeProduct = _productFaker.Generate();

            _productsServiceMock.Setup(service => service.GetAsync(fakeProduct.Id))
                                .ReturnsAsync(fakeProduct);
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<FavoriteProduct>>()))
                                    .ReturnsAsync(_fakeFavoriteProducts);

            // Act
            await _favoritesService.CreateAsync(fakeProduct.Id, _userId);

            // Assert
            _favoritesRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<FavoriteProduct>(fp => fp.Product.Id == fakeProduct.Id && fp.UserId == _userId)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenProductInFavorites_NotAddProductToFavorites()
        {
            // Arrange
            var fakeProduct = _fakeFavoriteProducts.First().Product;

            _productsServiceMock.Setup(service => service.GetAsync(fakeProduct.Id))
                                .ReturnsAsync(fakeProduct);
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<FavoriteProduct>>()))
                                    .ReturnsAsync(_fakeFavoriteProducts);

            // Act
            await _favoritesService.CreateAsync(fakeProduct.Id, _userId);

            // Assert
            _favoritesRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<FavoriteProduct>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_DeletesFavoriteProduct()
        {
            // Arrange
            var favoriteId = _fakeFavoriteProducts.First().Id;
            _favoritesRepositoryMock.Setup(repo => repo.DeleteAsync(favoriteId))
                                    .Returns(Task.CompletedTask);

            // Act
            await _favoritesService.DeleteAsync(favoriteId);

            // Assert
            _favoritesRepositoryMock.Verify(repo => repo.DeleteAsync(favoriteId), Times.Once);
        }

        [Fact]
        public async Task DeleteAllAsync_WhenCalled_DeletesAllFavoritesForUser()
        {
            // Arrange
            _favoritesRepositoryMock.Setup(repo => repo.DeleteAllAsync(_userId))
                                    .Returns(Task.CompletedTask);

            // Act
            await _favoritesService.DeleteAllAsync(_userId);

            // Assert
            _favoritesRepositoryMock.Verify(repo => repo.DeleteAllAsync(_userId), Times.Once);
        }
    }
}
