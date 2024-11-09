//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using OnlineShop.Db.Models;
//using OnlineShopWebApp.Controllers;
//using OnlineShopWebApp.Models;
//using OnlineShopWebApp.Services;
//using OnlineShopWebApp.Tests.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace OnlineShopWebApp.Tests.Controllers
//{
//    public class FavoriteControllerTests
//    {
//        private readonly Mock<FavoritesService> _favoritesServiceMock;
//        private readonly FavoriteController _controller;
//        private readonly IMapper _mapper;
//        private readonly List<FavoriteProduct> _fakeFavoriteProducts;

//        public FavoriteControllerTests()
//        {
//            _favoritesServiceMock = new Mock<FavoritesService>(null!, null!, null!);
//            _controller = new FavoriteController(_favoritesServiceMock.Object);

//            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
//            _mapper = config.CreateMapper();

//            _fakeFavoriteProducts = FakerProvider.FakeFavoriteProducts;
//        }

//        [Fact]
//        public async Task Index_WhenCalled_ReturnsViewWithFavorites()
//        {
//            // Arrange
//            var expectedFavorites = _fakeFavoriteProducts.Select(_mapper.Map<FavoriteProductViewModel>)
//                                                         .ToList();
//            _favoritesServiceMock.Setup(s => s.GetAllAsync(It.IsAny<string>()))
//                                 .ReturnsAsync(expectedFavorites);

//            // Act
//            var result = await _controller.Index();

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            Assert.Equal(expectedFavorites, viewResult.Model);
//        }

//        [Fact]
//        public async Task Add_AddsProductToFavorites_ReturnsPartialView()
//        {
//            // Arrange
//            var productId = Guid.NewGuid();

//            // Act
//            var result = await _controller.Add(productId);

//            // Assert
//            _favoritesServiceMock.Verify(s => s.CreateAsync(productId, It.IsAny<string>()), Times.Once);
//            var partialViewResult = Assert.IsType<PartialViewResult>(result);
//            Assert.Equal("_NavUserIcons", partialViewResult.ViewName);
//        }

//        [Fact]
//        public async Task Delete_RemovesProductFromFavorites_ReturnsRedirectToIndex()
//        {
//            // Arrange
//            var favoriteId = Guid.NewGuid();

//            // Act
//            var result = await _controller.Delete(favoriteId);

//            // Assert
//            _favoritesServiceMock.Verify(s => s.DeleteAsync(favoriteId), Times.Once);
//            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//            Assert.Equal("Index", redirectResult.ActionName);
//        }

//        [Fact]
//        public async Task DeleteAll_RemovesAllFavoritesForUser_ReturnsRedirectToIndex()
//        {
//            // Act
//            var result = await _controller.DeleteAll();

//            // Assert
//            _favoritesServiceMock.Verify(s => s.DeleteAllAsync(It.IsAny<string>()), Times.Once);
//            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//            Assert.Equal("Index", redirectResult.ActionName);
//        }
//    }
//}
