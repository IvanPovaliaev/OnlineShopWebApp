using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<AccountsService> _accountsServiceMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        private readonly AccountController _controller;

        private const int RolesCount = 10;
        private const int UsersCount = 10;
        private readonly List<Role> _fakeRoles;
        private readonly List<User> _fakeUsers;

        public AccountControllerTests()
        {
            _accountsServiceMock = new Mock<AccountsService>(null!, null!, null!, null!, null!);
            _urlHelperMock = new Mock<IUrlHelper>();
            _controller = new AccountController(_accountsServiceMock.Object)
            {
                Url = _urlHelperMock.Object
            };

            _fakeRoles = FakerProvider.RoleFaker.Generate(RolesCount);

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password(12))
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Role, f => f.PickRandom(_fakeRoles));

            _fakeUsers = userFaker.Generate(UsersCount);
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
            var result = await _controller.Login(loginModel, false);

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
            var result = await _controller.Login(loginModel, true);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
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
