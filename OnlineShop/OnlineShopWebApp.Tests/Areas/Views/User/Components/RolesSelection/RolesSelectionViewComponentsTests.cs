using AutoMapper;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShop.Db;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Areas.Admin.Views.User.Components.RolesSelection;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Areas.Views.User.Components.RolesSelection
{
    public class RolesSelectionViewComponentTests
    {
        private readonly Mock<RolesService> _rolesServiceMock;
        private readonly RolesSelectionViewComponent _viewComponent;
        private readonly List<RoleViewModel> _fakeRoles;

        public RolesSelectionViewComponentTests(IMapper mapper, FakerProvider fakerProvider)
        {
            _rolesServiceMock = new Mock<RolesService>(null!, null!, null!, null!);
            _viewComponent = new RolesSelectionViewComponent(_rolesServiceMock.Object);

            _fakeRoles = fakerProvider.FakeRoles.Select(mapper.Map<RoleViewModel>)
                                                .ToList();
        }

        [Fact]
        public async Task InvokeAsync_WhenSelectedRoleExists_ReturnsViewWithSelectedRoleName()
        {
            // Arrange
            var selectedRoleName = _fakeRoles.First().Name;

            _rolesServiceMock.Setup(r => r.GetAllAsync())
                             .ReturnsAsync(_fakeRoles);

            // Act
            var result = await _viewComponent.InvokeAsync(selectedRoleName);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("RolesSelection", viewResult.ViewName);

            var model = Assert.IsType<(List<RoleViewModel>, string)>(viewResult.ViewData!.Model);
            Assert.Equal(_fakeRoles, model.Item1);
            Assert.Equal(selectedRoleName, model.Item2);
        }

        [Fact]
        public async Task InvokeAsync_WhenSelectedRoleNotExists_ReturnsViewWithDefaultUserRole()
        {
            // Arrange
            var nonExistentRoleName = Guid.NewGuid().ToString();
            var defaultRoleName = Constants.UserRoleName;

            _rolesServiceMock.Setup(r => r.GetAllAsync())
                             .ReturnsAsync(_fakeRoles);

            // Act
            var result = await _viewComponent.InvokeAsync(nonExistentRoleName);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("RolesSelection", viewResult.ViewName);

            var model = Assert.IsType<(List<RoleViewModel>, string)>(viewResult.ViewData!.Model);
            Assert.Equal(_fakeRoles, model.Item1);
            Assert.Equal(defaultRoleName, model.Item2);
        }
    }
}
