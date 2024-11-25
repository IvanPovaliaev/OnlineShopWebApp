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
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly IMapper _mapper;
        private readonly AccountController _controller;
        private readonly List<Order> _fakeOrders;
        private readonly List<User> _fakeUsers;
        private readonly Faker<User> _userFaker;

        public AccountControllerTests(FakerProvider fakerProvider, Mock<IAccountsService> accountsServiceMock, Mock<IOrdersService> ordersServiceMock, Mock<IUrlHelper> urlHelperMock, Mock<IMediator> mediatorMock, Mock<IMailService> mailServiceMock, Mock<IHttpContextAccessor> _httpContextAccessor, IMapper mapper)
        {
            _userId = fakerProvider.UserId;

            _accountsServiceMock = accountsServiceMock;
            _ordersServiceMock = ordersServiceMock;
            _urlHelperMock = urlHelperMock;
            _mediatorMock = mediatorMock;
            _mailServiceMock = mailServiceMock;

            _mapper = mapper;

            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Scheme)
                       .Returns("https");

            httpContextMock.Setup(c => c.Request)
                           .Returns(requestMock.Object);

            _controller = new AccountController(_accountsServiceMock.Object, _ordersServiceMock.Object, _mediatorMock.Object, _mailServiceMock.Object, _httpContextAccessor.Object)
            {
                Url = _urlHelperMock.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
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

        [Fact]
        public async Task ForgotPassword_WhenModelIsInvalid_ReturnsForgotPasswordForm()
        {
            // Arrange
            var forgotModel = new ForgotPasswordViewModel
            {
                Email = _fakeUsers.First().Email!,
            };

            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsForgotPasswordValidAsync(modelState, forgotModel))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.ForgotPassword(forgotModel);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_ForgotPasswordForm", partialViewResult.ViewName);
            _accountsServiceMock.Verify(s => s.GetPasswordResetTokenAsync(forgotModel.Email), Times.Never);
            _mailServiceMock.Verify(s => s.SendEmailAsync(forgotModel.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPassword_WhenModelIsValid_SendMailAndReturnsRedirectUrl()
        {
            // Arrange
            var forgotModel = new ForgotPasswordViewModel
            {
                Email = _fakeUsers.First().Email!,
            };

            var token = new Faker().Random.AlphaNumeric(32);

            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsForgotPasswordValidAsync(modelState, forgotModel))
                                .ReturnsAsync(true);

            _accountsServiceMock.Setup(s => s.GetPasswordResetTokenAsync(forgotModel.Email))
                                .ReturnsAsync(token);

            _mailServiceMock.Setup(s => s.SendEmailAsync(forgotModel.Email, It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ForgotPassword(forgotModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            _accountsServiceMock.Verify(s => s.GetPasswordResetTokenAsync(forgotModel.Email), Times.Once);
            _mailServiceMock.Verify(s => s.SendEmailAsync(forgotModel.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SendResetPassword_WhenCalled_ReturnsView()
        {
            // Act
            var result = _controller.SendResetPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void ResetPassword_WhenCalled_ReturnsViewWithModel()
        {
            // Arrange
            var email = _fakeUsers.First().Email;
            var token = new Faker().Random.AlphaNumeric(32);

            // Act
            var result = _controller.ResetPassword(email, token);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
            Assert.Equal(email, model.Email);
            Assert.Equal(token, model.Token);
        }

        [Fact]
        public async Task ResetPassword_WhenModelIsInvalid_ReturnsViewWithModel()
        {
            // Arrange
            var email = _fakeUsers.First().Email;
            var token = new Faker().Random.AlphaNumeric(32);

            var resetModel = new ResetPasswordViewModel
            {
                Email = email,
                Token = token,
                Password = "newpassword",
                ConfirmPassword = "newpassword"
            };

            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsResetPasswordValidAsync(modelState, resetModel))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.ResetPassword(resetModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(resetModel, viewResult.Model);
        }

        [Fact]
        public async Task ResetPassword_WhenModelIsValidButResetFails_ReturnsErrorView()
        {
            // Arrange
            var email = _fakeUsers.First().Email;
            var token = new Faker().Random.AlphaNumeric(32);

            var resetModel = new ResetPasswordViewModel
            {
                Email = email,
                Token = token,
                Password = "newpassword",
                ConfirmPassword = "newpassword"
            };

            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsResetPasswordValidAsync(modelState, resetModel))
                                .ReturnsAsync(true);
            _accountsServiceMock.Setup(s => s.TryResetPassword(resetModel))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.ResetPassword(resetModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            _accountsServiceMock.Verify(s => s.TryResetPassword(resetModel), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_WhenModelIsValidAndResetSucceeds_SendsEmailAndReturnsSuccessView()
        {
            // Arrange
            var email = _fakeUsers.First().Email;
            var token = new Faker().Random.AlphaNumeric(32);

            var resetModel = new ResetPasswordViewModel
            {
                Email = email,
                Token = token,
                Password = "newpassword",
                ConfirmPassword = "newpassword"
            };

            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsResetPasswordValidAsync(modelState, resetModel))
                                .ReturnsAsync(true);
            _accountsServiceMock.Setup(s => s.TryResetPassword(resetModel))
                                .ReturnsAsync(true);
            _mailServiceMock.Setup(s => s.SendEmailAsync(resetModel.Email!, It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ResetPassword(resetModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.SuccessResetPassword), redirectToActionResult.ActionName);
            _accountsServiceMock.Verify(s => s.TryResetPassword(resetModel), Times.Once);
            _mailServiceMock.Verify(s => s.SendEmailAsync(resetModel.Email!, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SuccessResetPassword_WhenCalled_ReturnsView()
        {
            // Act
            var result = _controller.SuccessResetPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }
    }
}
