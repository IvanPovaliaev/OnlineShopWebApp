using AutoMapper;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using OnlineShopWebApp.Views.Shared.Components.Cart;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Views.Components.Cart
{
    public class CartViewComponentTests
    {
        private readonly Mock<CartsService> _cartsServiceMock;
        private readonly CartViewComponent _viewComponent;
        private readonly CartViewModel _fakeCartViewModel;

        public CartViewComponentTests()
        {
            _cartsServiceMock = new Mock<CartsService>(null!, null!, null!);
            _viewComponent = new CartViewComponent(_cartsServiceMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            var mapper = config.CreateMapper();

            _fakeCartViewModel = mapper.Map<CartViewModel>(FakerProvider.FakeCart);
        }

        [Fact]
        public async Task InvokeAsync_WhenCartHasItems_ReturnsCorrectTotalQuantityInView()
        {
            // Arrange
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(_fakeCartViewModel);
            var expectedItemQuantity = _fakeCartViewModel.TotalQuantity;

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(expectedItemQuantity, factQuantity);
        }

        [Fact]
        public async Task InvokeAsync_WhenCartIsEmpty_ReturnsZeroTotalQuantityInView()
        {
            // Arrange
            var fakeCart = new CartViewModel();
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(fakeCart);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
        }

        [Fact]
        public async Task InvokeAsync_WhenCartIsNull_ReturnsZeroTotalQuantityInView()
        {
            // Arrange
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(It.IsAny<Guid>()))
                             .ReturnsAsync((CartViewModel)null!);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
        }
    }
}
