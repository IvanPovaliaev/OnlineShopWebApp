using AutoMapper;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using OnlineShopWebApp.Views.Shared.Components.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Views.Components.Favorite
{
    public class FavoriteViewComponentTests
    {
        private readonly Mock<FavoritesService> _favoritesServiceMock;
        private readonly FavoriteViewComponent _viewComponent;
        private readonly List<FavoriteProductViewModel> _fakeFavoriteProducts;

        public FavoriteViewComponentTests()
        {
            _favoritesServiceMock = new Mock<FavoritesService>(null!, null!, null!);
            _viewComponent = new FavoriteViewComponent(_favoritesServiceMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            var mapper = config.CreateMapper();

            _fakeFavoriteProducts = FakerProvider.FakeFavoriteProducts.Select(mapper.Map<FavoriteProductViewModel>)
                                                              .ToList();
        }

        [Fact]
        public async Task InvokeAsync_WhenFavoritesExist_ReturnsCorrectCountInView()
        {
            // Arrange
            var expectedQuantity = _fakeFavoriteProducts.Count;
            _favoritesServiceMock.Setup(s => s.GetAllAsync(It.IsAny<string>()))
                                 .ReturnsAsync(_fakeFavoriteProducts);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Favorite", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(expectedQuantity, factQuantity);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoFavorites_ReturnsZeroCountInView()
        {
            // Arrange
            var emptyFavorites = new List<FavoriteProductViewModel>();
            _favoritesServiceMock.Setup(s => s.GetAllAsync(It.IsAny<string>()))
                                 .ReturnsAsync(emptyFavorites);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Favorite", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
        }
    }
}
