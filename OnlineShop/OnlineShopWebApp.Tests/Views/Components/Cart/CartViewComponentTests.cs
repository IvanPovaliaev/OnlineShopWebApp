using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Helpers;
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
        private readonly string? _userId;
        private readonly Mock<CartsService> _cartsServiceMock;
        private readonly Mock<CookieCartsService> _cookieCartsServiceMock;
        private readonly CartViewComponent _viewComponent;
        private readonly CartViewModel _fakeCartViewModel;

        public CartViewComponentTests(IMapper mapper, FakerProvider fakerProvider, Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            _userId = fakerProvider.UserId;

            _cartsServiceMock = new Mock<CartsService>(null!, null!, null!);
            _cookieCartsServiceMock = new Mock<CookieCartsService>(null!, null!, null!);
            var authenticationHelper = new Mock<AuthenticationHelper>(null!);

            _viewComponent = new CartViewComponent(_cartsServiceMock.Object, _cookieCartsServiceMock.Object, httpContextAccessorMock.Object, authenticationHelper.Object);

            _fakeCartViewModel = mapper.Map<CartViewModel>(fakerProvider.FakeCart);
        }

        [Fact]
        public async Task InvokeAsync_WhenCartHasItems_ReturnsCorrectTotalQuantityInView()
        {
            // Arrange
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(_userId!))
                             .ReturnsAsync(_fakeCartViewModel);
            var expectedItemQuantity = _fakeCartViewModel.TotalQuantity;

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(expectedItemQuantity, factQuantity);
            _cartsServiceMock.Verify(s => s.GetViewModelAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenCartIsEmpty_ReturnsZeroTotalQuantityInView()
        {
            // Arrange
            var fakeCart = new CartViewModel();
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(_userId!))
                             .ReturnsAsync(fakeCart);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
            _cartsServiceMock.Verify(s => s.GetViewModelAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenCartIsNull_ReturnsZeroTotalQuantityInView()
        {
            // Arrange
            _cartsServiceMock.Setup(s => s.GetViewModelAsync(_userId!))
                             .ReturnsAsync((CartViewModel)null!);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
            _cartsServiceMock.Verify(s => s.GetViewModelAsync(_userId!), Times.Once);
        }
    }
}
