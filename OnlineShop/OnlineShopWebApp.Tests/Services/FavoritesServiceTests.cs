using AutoMapper;
using Bogus;
using Moq;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
{
    public class FavoritesServiceTests
    {
        private readonly Mock<IFavoritesRepository> _favoritesRepositoryMock;
        private readonly Mock<ProductsService> _productsServiceMock;
        private readonly FavoritesService _favoritesService;
        private readonly IMapper _mapper;

        private const int ProductsCount = 10;
        private readonly Guid _userId;
        private readonly List<FavoriteProduct> _fakeFavoriteProducts;
        private readonly Faker<Product> _productsFaker;

        public FavoritesServiceTests()
        {
            _favoritesRepositoryMock = new Mock<IFavoritesRepository>();
            _productsServiceMock = new Mock<ProductsService>(null!, null!, null!, null!);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _favoritesService = new FavoritesService(
                _favoritesRepositoryMock.Object,
                _mapper,
                _productsServiceMock.Object);

            _userId = Guid.NewGuid();

            _productsFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Cost, f => f.Finance.Amount())
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Category, f => f.PickRandom<ProductCategories>())
                .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl());

            _fakeFavoriteProducts = new Faker<FavoriteProduct>()
                .RuleFor(fp => fp.Id, f => Guid.NewGuid())
                .RuleFor(fp => fp.UserId, f => _userId)
                .RuleFor(fp => fp.Product, f => _productsFaker.Generate())
                .Generate(ProductsCount);
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasFavorites_ReturnsUserFavorites()
        {
            // Arrange
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync())
                                    .ReturnsAsync(_fakeFavoriteProducts);

            // Act
            var result = await _favoritesService.GetAllAsync(_userId);

            // Assert
            Assert.Equal(ProductsCount, result.Count);
            Assert.All(result, favorite => Assert.IsType<FavoriteProductViewModel>(favorite));
            Assert.All(result, favorite => Assert.Equal(_userId, favorite.UserId));
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasNoFavorites_ReturnsEmpty()
        {
            // Arrange
            var anotherUserId = Guid.NewGuid();
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync())
                                    .ReturnsAsync(_fakeFavoriteProducts);

            // Act
            var result = await _favoritesService.GetAllAsync(anotherUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_WhenProductIsNotInFavorites_AddProductToFavorites()
        {
            // Arrange
            var fakeProduct = _productsFaker.Generate();

            _productsServiceMock.Setup(service => service.GetAsync(fakeProduct.Id))
                                .ReturnsAsync(fakeProduct);
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync())
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
            _favoritesRepositoryMock.Setup(repo => repo.GetAllAsync())
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
