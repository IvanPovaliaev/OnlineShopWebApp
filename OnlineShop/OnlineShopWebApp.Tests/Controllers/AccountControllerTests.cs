using AutoMapper;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly string? _userId;
        private readonly Mock<IAccountsService> _accountsServiceMock;
        private readonly Mock<IOrdersService> _ordersServiceMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly IMapper _mapper;
        private readonly AccountController _controller;
        private readonly List<Order> _fakeOrders;
        private readonly List<User> _fakeUsers;
        private readonly Faker<User> _userFaker;

        public AccountControllerTests(FakerProvider fakerProvider, Mock<IAccountsService> accountsServiceMock, Mock<IOrdersService> ordersServiceMock, Mock<IUrlHelper> urlHelperMock, Mock<IMediator> mediatorMock, Mock<IHttpContextAccessor> _httpContextAccessor, IMapper mapper)
        {
            _userId = fakerProvider.UserId;

            _accountsServiceMock = accountsServiceMock;
            _ordersServiceMock = ordersServiceMock;
            _urlHelperMock = urlHelperMock;
            _mediatorMock = mediatorMock;

            _mapper = mapper;

            _controller = new AccountController(_accountsServiceMock.Object, _ordersServiceMock.Object, _mediatorMock.Object, _httpContextAccessor.Object)
            {
                Url = _urlHelperMock.Object
            };

            _fakeOrders = fakerProvider.FakeOrders;
            _fakeUsers = fakerProvider.FakeUsers;
            _userFaker = fakerProvider.UserFaker;
        }

        [Fact]
        public void Unauthorized_WhenCalled_ReturnsUnauthorizedView()
        {
            // Arrange
            var returnUrl = "/some/page";

            // Act
            var result = _controller.Unauthorized(returnUrl);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_WhenUserIdIsNull_ReturnsViewWithEditUserViewModel()
        {
            // Arrange
            var editUserViewModel = new EditUserViewModel { Id = _userId! };
            var editNullUserViewModel = new EditUserViewModel { Id = null! };

            _accountsServiceMock.Setup(s => s.GetEditViewModelAsync(_userId!))
                                .ReturnsAsync(editNullUserViewModel);

            // Act
            var result = await _controller.Index(editNullUserViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editNullUserViewModel, viewResult.Model);
            _accountsServiceMock.Verify(s => s.GetEditViewModelAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task Index_WhenUserIdIsNotNull_ReturnsViewWithEditUserViewModel()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var editUserViewModel = new EditUserViewModel { Id = userId };

            // Act
            var result = await _controller.Index(editUserViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editUserViewModel, viewResult.Model);
            _accountsServiceMock.Verify(s => s.GetEditViewModelAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenModelIsInvalid_ReturnsIndexViewWithEditUserViewModel()
        {
            // Arrange
            var user = _userFaker.Generate();
            var editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email!
            };

            var modelState = new ModelStateDictionary();

            _accountsServiceMock.Setup(s => s.IsEditUserValidAsync(modelState, editUserViewModel))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(editUserViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(editUserViewModel, viewResult.Model);
        }

        [Fact]
        public async Task Update_WhenModelIsValid_UpdatesUserAndRedirectsToIndex()
        {
            // Arrange
            var user = _userFaker.Generate();
            var editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email!
            };

            var modelState = new ModelStateDictionary();

            _accountsServiceMock.Setup(s => s.IsEditUserValidAsync(modelState, editUserViewModel))
                                .ReturnsAsync(true);

            _accountsServiceMock.Setup(s => s.UpdateInfoAsync(editUserViewModel))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(editUserViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Orders_WhenCalled_ReturnsViewWithOrders()
        {
            // Arrange
            var orders = _fakeOrders.Select(_mapper.Map<OrderViewModel>)
                                    .ToList();

            _ordersServiceMock.Setup(s => s.GetAllAsync(_userId!))
                              .ReturnsAsync(orders);
            // Act
            var result = await _controller.Orders();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(orders, viewResult.Model);
        }

        [Fact]
        public async Task Login_WhenModelIsInvalid_ReturnsLoginForm()
        {
            // Arrange
            var loginModel = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "wrongpassword"
            };
            var modelState = new ModelStateDictionary();

            _accountsServiceMock.Setup(s => s.IsLoginValidAsync(modelState, loginModel))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_LoginForm", partialViewResult.ViewName);
        }

        [Fact]
        public async Task Login_WhenModelIsValid_ReturnsRedirectUrl()
        {
            var existingUser = _fakeUsers.First();
            // Arrange
            var loginModel = new LoginViewModel
            {
                Email = existingUser.Email,
                Password = "password123"
            };
            var modelState = new ModelStateDictionary();

            _accountsServiceMock.Setup(s => s.IsLoginValidAsync(modelState, loginModel))
                                .ReturnsAsync(true);

            _urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                          .Returns("/Home/Index");

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Logout_WhenCalled_RedirectToActionResult()
        {
            // Arrange
            _accountsServiceMock.Setup(s => s.LogoutAsync())
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            _accountsServiceMock.Verify(s => s.LogoutAsync(), Times.Once);
        }

        [Fact]
        public async Task Register_WhenModelIsInvalid_ReturnsRegistrationForm()
        {
            // Arrange
            var registerModel = new UserRegisterViewModel
            {
                Email = "newuser@example.com",
                Password = "short"
            };

            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsRegisterValidAsync(modelState, registerModel))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_RegistrationForm", partialViewResult.ViewName);
        }

        [Fact]
        public async Task Register_WhenModelIsValid_CreatesUserAndReturnsRedirectUrl()
        {
            // Arrange
            var registerModel = new UserRegisterViewModel
            {
                Email = "newuser@example.com",
                Password = "validpassword123"
            };
            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsRegisterValidAsync(modelState, registerModel))
                                .ReturnsAsync(true);

            _accountsServiceMock.Setup(s => s.AddAsync(registerModel))
                                .Returns(Task.CompletedTask);

            _urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
              .Returns("/Home/Index");

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
        }
    }
}
