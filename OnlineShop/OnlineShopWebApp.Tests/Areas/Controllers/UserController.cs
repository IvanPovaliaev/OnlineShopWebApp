using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Controllers;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Areas.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<AccountsService> _accountsServiceMock;
        private readonly UserController _controller;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        private readonly IMapper _mapper;

        private readonly List<User> _fakeUsers;
        private readonly Faker<User> _userFaker;

        public UserControllerTests()
        {
            _accountsServiceMock = new Mock<AccountsService>(null!, null!, null!, null!, null!);
            _urlHelperMock = new Mock<IUrlHelper>();

            _controller = new UserController(_accountsServiceMock.Object)
            {
                Url = _urlHelperMock.Object
            };

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _userFaker = FakerProvider.UserFaker;
            _fakeUsers = FakerProvider.FakeUsers;
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithUsers()
        {
            // Arrange
            var fakeUsers = _fakeUsers.Select(_mapper.Map<UserViewModel>)
                                      .ToList();
            _accountsServiceMock.Setup(s => s.GetAllAsync())
                                .ReturnsAsync(fakeUsers);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(viewResult.Model);
            Assert.Equal(fakeUsers, model);
        }

        [Fact]
        public void Add_WhenCalled_ReturnsView()
        {
            // Act
            var result = _controller.Add();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var invalidRegisterModel = new AdminRegisterViewModel();
            _controller.ModelState.AddModelError("Username", "Required");

            // Act
            var result = await _controller.Add(invalidRegisterModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Add", viewResult.ViewName);
            Assert.Equal(invalidRegisterModel, viewResult.Model);
        }

        [Fact]
        public async Task Add_WithValidModel_AddsUserAndRedirectsToIndex()
        {
            // Arrange
            var newFakeUser = _userFaker.Generate();
            var validRegisterModel = new AdminRegisterViewModel()
            {
                Email = newFakeUser.Email,
                Password = newFakeUser.Password,
                ConfirmPassword = newFakeUser.Password,
                Name = newFakeUser.Name,
                Phone = newFakeUser.Phone,
                RoleId = newFakeUser.Role.Id
            };

            var modelState = new ModelStateDictionary();
            _accountsServiceMock.Setup(s => s.IsRegisterValidAsync(modelState, validRegisterModel))
                                .ReturnsAsync(true);

            // Act
            var result = await _controller.Add(validRegisterModel);

            // Assert
            _accountsServiceMock.Verify(s => s.AddAsync(validRegisterModel), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Details_WhenCalled_ReturnsViewWithUser()
        {
            // Arrange
            var fakeUser = _mapper.Map<UserViewModel>(_fakeUsers.First());
            _accountsServiceMock.Setup(s => s.GetAsync(fakeUser.Id))
                                .ReturnsAsync(fakeUser);

            // Act
            var result = await _controller.Details(fakeUser.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(fakeUser, model);
        }

        [Fact]
        public async Task Details_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _accountsServiceMock.Setup(s => s.GetAsync(userId))
                                .ReturnsAsync((UserViewModel)null!);

            // Act
            var result = await _controller.Details(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ChangePassword_WithInvalidModel_ReturnsPartialViewWithModel()
        {
            // Arrange
            var invalidChangePasswordModel = new ChangePasswordViewModel();
            _controller.ModelState.AddModelError("Password", "Required");

            // Act
            var result = await _controller.ChangePassword(invalidChangePasswordModel);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_ChangePasswordForm", partialViewResult.ViewName);
            Assert.Equal(invalidChangePasswordModel, partialViewResult.Model);
        }

        [Fact]
        public async Task ChangePassword_WithValidModel_ChangesPasswordAndReturnsRedirectJson()
        {
            // Arrange
            var validChangePasswordModel = new ChangePasswordViewModel()
            {
                UserId = _fakeUsers.First().Id,
                Password = "newPassword123!",
                ConfirmPassword = "newPassword123!"
            };

            _urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                          .Returns(string.Empty);

            // Act
            var result = await _controller.ChangePassword(validChangePasswordModel);

            // Assert
            _accountsServiceMock.Verify(s => s.ChangePasswordAsync(validChangePasswordModel), Times.Once);
            var jsonResult = Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void Edit_WhenCalled_ReturnsViewWithModel()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var editUserModel = new AdminEditUserViewModel()
            {
                UserId = fakeUser.Id,
                Email = fakeUser.Email,
                Phone = fakeUser.Phone,
                Name = fakeUser.Name,
                RoleId = fakeUser.Role.Id
            };

            // Act
            var result = _controller.Edit(editUserModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editUserModel, viewResult.Model);
        }

        [Fact]
        public async Task Update_WithInvalidModel_ReturnsEditViewWithModel()
        {
            // Arrange
            var invalidEditUserModel = new AdminEditUserViewModel();
            var modelState = _controller.ModelState;
            modelState.AddModelError("Username", "Required");
            _accountsServiceMock.Setup(s => s.IsEditUserValidAsync(modelState, invalidEditUserModel))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(invalidEditUserModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Edit", viewResult.ViewName);
            Assert.Equal(invalidEditUserModel, viewResult.Model);
        }

        [Fact]
        public async Task Update_WithValidModel_UpdatesUserAndRedirectsToDetails()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var validEditUserModel = new AdminEditUserViewModel()
            {
                UserId = fakeUser.Id,
                Email = fakeUser.Email,
                Phone = fakeUser.Phone,
                Name = new Faker().Name.FullName(),
                RoleId = fakeUser.Role.Id
            };
            var modelState = _controller.ModelState;
            _accountsServiceMock.Setup(s => s.IsEditUserValidAsync(modelState, validEditUserModel))
                                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(validEditUserModel);

            // Assert
            _accountsServiceMock.Verify(s => s.UpdateInfoAsync(validEditUserModel), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_WhenCalled_DeletesUserAndRedirectsToIndex()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            _accountsServiceMock.Verify(s => s.DeleteAsync(userId), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task ExportToExcel_WhenCalled_ReturnsExcelFileStream()
        {
            // Arrange
            var fakeStream = new MemoryStream();
            _accountsServiceMock.Setup(s => s.ExportAllToExcelAsync())
                                .ReturnsAsync(fakeStream);

            // Act
            var result = await _controller.ExportToExcel();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(Constants.ExcelFileContentType, fileResult.ContentType);
        }
    }
}
