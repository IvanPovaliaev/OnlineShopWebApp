using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
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
        private readonly Mock<ICartsService> _cartsServiceMock;
        private readonly Mock<ICookieCartsService> _cookieCartsServiceMock;
        private readonly Mock<AuthenticationHelper> _authenticationHelperMock;
        private readonly CartViewComponent _viewComponent;
        private readonly CartViewModel _fakeCartViewModel;

        public CartViewComponentTests(Mock<ICartsService> cartsServiceMock, Mock<ICookieCartsService> cookieCartsServiceMock, IMapper mapper, FakerProvider fakerProvider, Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            _userId = fakerProvider.UserId;

            _cartsServiceMock = cartsServiceMock;
            _cookieCartsServiceMock = cookieCartsServiceMock;
            _authenticationHelperMock = new Mock<AuthenticationHelper>(null!);

            _viewComponent = new CartViewComponent(_cartsServiceMock.Object, _cookieCartsServiceMock.Object, httpContextAccessorMock.Object, _authenticationHelperMock.Object);

            _fakeCartViewModel = mapper.Map<CartViewModel>(fakerProvider.FakeCart);
        }

        [Fact]
        public async Task InvokeAsync_WhenCartHasItems_ReturnsCorrectTotalQuantityInView()
        {
            // Arrange
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()))
                                     .ReturnsAsync(_fakeCartViewModel);
            var expectedItemQuantity = _fakeCartViewModel.TotalQuantity;

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(expectedItemQuantity, factQuantity);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()), Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_WhenCartIsEmpty_ReturnsZeroTotalQuantityInView()
        {
            // Arrange
            var fakeCart = new CartViewModel();
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()))
                                     .ReturnsAsync(fakeCart);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()), Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_WhenCartIsNull_ReturnsZeroTotalQuantityInView()
        {
            // Arrange
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()))
                         .ReturnsAsync((CartViewModel)null!);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Cart", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()), Times.Once());
        }
    }
}
