using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class CartControllerTests
    {
        private readonly Mock<CartsService> _cartsServiceMock;
        private readonly CartController _controller;
        private readonly Guid _userId;
        private readonly IMapper _mapper;
        private readonly CartViewModel _fakeCartViewModel;

        public CartControllerTests()
        {
            _cartsServiceMock = new Mock<CartsService>(null!, null!, null!);
            _controller = new CartController(_cartsServiceMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _userId = FakerProvider.UserId;
            _fakeCartViewModel = new CartViewModel();
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithCart()
        {
            // Arrange
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(_fakeCartViewModel);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CartViewModel>(viewResult.Model);
            Assert.Equal(_fakeCartViewModel, model);
        }

        [Fact]
        public async Task Add_AddsProductToCart_ReturnsPartialView()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.Add(productId);

            // Assert
            _cartsServiceMock.Verify(s => s.AddAsync(productId, It.IsAny<Guid>()), Times.Once);
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_NavUserIcons", partialViewResult.ViewName);
        }

        [Fact]
        public async Task Increase_IncreasesProductQuantity_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();

            // Act
            var result = await _controller.Increase(positionId);

            // Assert
            _cartsServiceMock.Verify(s => s.IncreasePositionAsync(It.IsAny<Guid>(), positionId), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Decrease_DecreasesProductQuantity_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();

            // Act
            var result = await _controller.Decrease(positionId);

            // Assert
            _cartsServiceMock.Verify(s => s.DecreasePosition(It.IsAny<Guid>(), positionId), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_RemovesAllProductsFromCart_ReturnsRedirectToIndex()
        {
            // Act
            var result = await _controller.Delete();

            // Assert
            _cartsServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeletePosition_RemovesProductFromCart_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();

            // Act
            var result = await _controller.DeletePosition(positionId);

            // Assert
            _cartsServiceMock.Verify(s => s.DeletePositionAsync(It.IsAny<Guid>(), positionId), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
