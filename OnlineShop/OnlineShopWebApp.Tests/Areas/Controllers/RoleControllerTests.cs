using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain.Models;
using OnlineShopWebApp.Areas.Admin.Controllers;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Tests.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Areas.Controllers
{
    public class RoleControllerTests
    {
        private readonly Mock<IRolesService> _rolesServiceMock;
        private readonly RoleController _controller;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        private readonly IMapper _mapper;

        private readonly Faker<Role> _roleFaker;
        private readonly List<Role> _fakeRoles;

        public RoleControllerTests(Mock<IRolesService> rolesServiceMock, FakerProvider fakerProvider)
        {
            _rolesServiceMock = rolesServiceMock;
            _urlHelperMock = new Mock<IUrlHelper>();

            _controller = new RoleController(_rolesServiceMock.Object)
            {
                Url = _urlHelperMock.Object
            };

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _roleFaker = fakerProvider.RoleFaker;
            _fakeRoles = fakerProvider.FakeRoles;
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithRoles()
        {
            // Arrange
            var fakeRoles = _fakeRoles.Select(_mapper.Map<RoleViewModel>)
                                      .ToList();
            _rolesServiceMock.Setup(s => s.GetAllAsync())
                             .ReturnsAsync(fakeRoles);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<RoleViewModel>>(viewResult.Model);
            Assert.Equal(fakeRoles, model);
        }

        [Fact]
        public async Task Add_WhenModelIsValid_AddsRoleAndReturnsJson()
        {
            // Arrange
            var newRole = _mapper.Map<AddRoleViewModel>(_roleFaker.Generate());
            var modelState = new ModelStateDictionary();
            _rolesServiceMock.Setup(s => s.IsNewValidAsync(modelState, newRole))
                             .ReturnsAsync(true);

            _urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                          .Returns("/Admin/Role/Index");

            // Act
            var result = await _controller.Add(newRole);

            // Assert
            _rolesServiceMock.Verify(s => s.AddAsync(newRole), Times.Once);
            var jsonResult = Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Add_WhenModelIsInvalid_ReturnsPartialViewWithRoleModel()
        {
            // Arrange
            var invalidRole = _mapper.Map<AddRoleViewModel>(_roleFaker.Generate());
            var modelState = new ModelStateDictionary();
            _rolesServiceMock.Setup(s => s.IsNewValidAsync(modelState, invalidRole))
                             .ReturnsAsync(false);

            // Act
            var result = await _controller.Add(invalidRole);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_AddForm", partialViewResult.ViewName);
            Assert.Equal(invalidRole, partialViewResult.Model);
        }

        [Fact]
        public async Task Delete_WhenCalled_DeletesRoleAndRedirectsToIndex()
        {
            // Arrange
            var roleName = _fakeRoles.First().Name;

            // Act
            var result = await _controller.Delete(roleName);

            // Assert
            _rolesServiceMock.Verify(s => s.DeleteAsync(roleName), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task ExportToExcel_WhenCalled_ReturnsExcelFileStream()
        {
            // Arrange
            var fakeStream = new MemoryStream();
            _rolesServiceMock.Setup(s => s.ExportAllToExcelAsync())
                             .ReturnsAsync(fakeStream);

            // Act
            var result = await _controller.ExportToExcel();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(Formats.ExcelFileContentType, fileResult.ContentType);
        }
    }
}
