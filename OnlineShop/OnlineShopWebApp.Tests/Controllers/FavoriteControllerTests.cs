using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class FavoriteControllerTests
    {
        private readonly string? _userId;
        private readonly Mock<IFavoritesService> _favoritesServiceMock;
        private readonly FavoriteController _controller;
        private readonly IMapper _mapper;
        private readonly List<FavoriteProduct> _fakeFavoriteProducts;

        public FavoriteControllerTests(IMapper mapper, Mock<IFavoritesService> favoritesServiceMock, Mock<IHttpContextAccessor> httpContextAccessorMock, FakerProvider fakerProvider)
        {
            _userId = fakerProvider.UserId;

            _favoritesServiceMock = favoritesServiceMock;
            _controller = new FavoriteController(_favoritesServiceMock.Object, httpContextAccessorMock.Object);

            _mapper = mapper;
            _fakeFavoriteProducts = fakerProvider.FakeFavoriteProducts;
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithFavorites()
        {
            // Arrange
            var expectedFavorites = _fakeFavoriteProducts.Select(_mapper.Map<FavoriteProductViewModel>)
                                                         .ToList();

            _favoritesServiceMock.Setup(s => s.GetAllAsync(_userId!))
                                 .ReturnsAsync(expectedFavorites);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expectedFavorites, viewResult.Model);
            _favoritesServiceMock.Verify(s => s.GetAllAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task Add_AddsProductToFavorites_ReturnsPartialView()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.Add(productId);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_NavUserIcons", partialViewResult.ViewName);
            _favoritesServiceMock.Verify(s => s.CreateAsync(productId, _userId!), Times.Once);
        }

        [Fact]
        public async Task Delete_RemovesProductFromFavorites_ReturnsRedirectToIndex()
        {
            // Arrange
            var favoriteId = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(favoriteId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _favoritesServiceMock.Verify(s => s.DeleteAsync(favoriteId), Times.Once);
        }

        [Fact]
        public async Task DeleteAll_RemovesAllFavoritesForUser_ReturnsRedirectToIndex()
        {
            // Act
            var result = await _controller.DeleteAll();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _favoritesServiceMock.Verify(s => s.DeleteAllAsync(_userId!), Times.Once);
        }
    }
}
