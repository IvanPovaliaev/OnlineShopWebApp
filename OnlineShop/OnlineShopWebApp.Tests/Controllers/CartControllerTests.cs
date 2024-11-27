using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class CartControllerTests
    {
        private readonly string? _userId;
        private readonly Mock<ICartsService> _cartsServiceMock;
        private readonly Mock<ICookieCartsService> _cookieCartsServiceMock;
        private readonly Mock<AuthenticationHelper> _authenticationHelperMock;
        private readonly CartController _controller;
        private readonly CartViewModel _fakeCartViewModel;

        public CartControllerTests(Mock<ICartsService> cartsServiceMock, Mock<ICookieCartsService> cookieCartsServiceMock, Mock<IHttpContextAccessor> httpContextAccessorMock, FakerProvider fakerProvider)
        {
            _userId = fakerProvider.UserId;

            _cartsServiceMock = cartsServiceMock;
            _cookieCartsServiceMock = cookieCartsServiceMock;
            _authenticationHelperMock = new Mock<AuthenticationHelper>(httpContextAccessorMock.Object);
            _controller = new CartController(_cartsServiceMock.Object, _cookieCartsServiceMock.Object, httpContextAccessorMock.Object, _authenticationHelperMock.Object);

            _fakeCartViewModel = new CartViewModel();
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithCart()
        {
            // Arrange
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()))
                                      .ReturnsAsync(_fakeCartViewModel);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CartViewModel>(viewResult.Model);

            Assert.Equal(_fakeCartViewModel, model);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task<CartViewModel>>>(), It.IsAny<Func<Task<CartViewModel>>>()), Times.Once());
        }

        [Fact]
        public async Task Add_WhenCalled_ReturnsPartialView()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()))
                                      .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(productId);

            // Assert            
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_NavUserIcons", partialViewResult.ViewName);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()), Times.Once());
        }

        [Fact]
        public async Task Increase_WhenCalled_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()))
                                      .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Increase(positionId);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()), Times.Once());
        }

        [Fact]
        public async Task Decrease_WhenCalled_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()))
                                      .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Decrease(positionId);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnsRedirectToIndex()
        {
            // Arrange
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Action>()))
                                      .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete();

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Action>()), Times.Once());
        }

        [Fact]
        public async Task DeletePosition_WhenCalled_ReturnsRedirectToIndex()
        {
            // Arrange
            var positionId = Guid.NewGuid();
            _authenticationHelperMock.Setup(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()))
                                      .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePosition(positionId);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _authenticationHelperMock.Verify(a => a.ExecuteBasedOnAuthenticationAsync(It.IsAny<Func<Task>>(), It.IsAny<Func<Task>>()), Times.Once());
        }
    }
}
