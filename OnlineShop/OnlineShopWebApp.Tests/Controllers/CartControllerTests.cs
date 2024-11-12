using Microsoft.AspNetCore.Http;
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
        private readonly string? _userId;
        private readonly Mock<CartsService> _cartsServiceMock;
        private readonly CartController _controller;
        private readonly CartViewModel _fakeCartViewModel;

        public CartControllerTests(Mock<IHttpContextAccessor> httpContextAccessorMock, FakerProvider fakerProvider)
        {
            _userId = fakerProvider.UserId;

            _cartsServiceMock = new Mock<CartsService>(null!, null!, null!);
            _controller = new CartController(_cartsServiceMock.Object, httpContextAccessorMock.Object);

            _fakeCartViewModel = new CartViewModel();
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithCart()
        {
            // Arrange
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(_userId!))
                             .ReturnsAsync(_fakeCartViewModel);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CartViewModel>(viewResult.Model);
            Assert.Equal(_fakeCartViewModel, model);
            _cartsServiceMock.Verify(s => s.GetViewModelAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task Add_AddsProductToCart_ReturnsPartialView()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.Add(productId);

            // Assert            
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_NavUserIcons", partialViewResult.ViewName);
            _cartsServiceMock.Verify(s => s.AddAsync(productId, _userId!), Times.Once);
        }

        [Fact]
        public async Task Increase_IncreasesProductQuantity_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();

            // Act
            var result = await _controller.Increase(positionId);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _cartsServiceMock.Verify(s => s.IncreasePositionAsync(_userId!, positionId), Times.Once);
        }

        [Fact]
        public async Task Decrease_DecreasesProductQuantity_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();

            // Act
            var result = await _controller.Decrease(positionId);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _cartsServiceMock.Verify(s => s.DecreasePosition(_userId!, positionId), Times.Once);
        }

        [Fact]
        public async Task Delete_RemovesAllProductsFromCart_ReturnsRedirectToIndex()
        {
            // Act
            var result = await _controller.Delete();

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _cartsServiceMock.Verify(s => s.DeleteAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task DeletePosition_RemovesProductFromCart_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();

            // Act
            var result = await _controller.DeletePosition(positionId);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _cartsServiceMock.Verify(s => s.DeletePositionAsync(_userId!, positionId), Times.Once);
        }
    }
}
