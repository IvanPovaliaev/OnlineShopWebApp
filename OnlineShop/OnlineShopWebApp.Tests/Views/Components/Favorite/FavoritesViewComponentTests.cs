using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
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
        private readonly string? _userId;
        private readonly Mock<IFavoritesService> _favoritesServiceMock;
        private readonly FavoriteViewComponent _viewComponent;
        private readonly List<FavoriteProductViewModel> _fakeFavoriteProducts;

        public FavoriteViewComponentTests(Mock<IFavoritesService> favoritesServiceMock, IMapper mapper, FakerProvider fakerProvider, Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            _userId = fakerProvider.UserId;
            _favoritesServiceMock = favoritesServiceMock;
            _viewComponent = new FavoriteViewComponent(_favoritesServiceMock.Object, httpContextAccessorMock.Object);

            _fakeFavoriteProducts = fakerProvider.FakeFavoriteProducts.Select(mapper.Map<FavoriteProductViewModel>)
                                                                      .ToList();
        }

        [Fact]
        public async Task InvokeAsync_WhenFavoritesExist_ReturnsCorrectCountInView()
        {
            // Arrange
            var expectedQuantity = _fakeFavoriteProducts.Count;
            _favoritesServiceMock.Setup(s => s.GetAllAsync(_userId!))
                                 .ReturnsAsync(_fakeFavoriteProducts);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Favorite", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(expectedQuantity, factQuantity);
            _favoritesServiceMock.Verify(s => s.GetAllAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoFavorites_ReturnsZeroCountInView()
        {
            // Arrange
            var emptyFavorites = new List<FavoriteProductViewModel>();
            _favoritesServiceMock.Setup(s => s.GetAllAsync(_userId!))
                                 .ReturnsAsync(emptyFavorites);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Favorite", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
            _favoritesServiceMock.Verify(s => s.GetAllAsync(_userId!), Times.Once);
        }
    }
}
