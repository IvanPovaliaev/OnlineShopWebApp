//using AutoMapper;
//using Microsoft.AspNetCore.Mvc.ViewComponents;
//using Moq;
//using OnlineShop.Db;
//using OnlineShopWebApp.Areas.Admin.Models;
//using OnlineShopWebApp.Areas.Admin.Views.User.Components.RolesSelection;
//using OnlineShopWebApp.Helpers;
//using OnlineShopWebApp.Services;
//using OnlineShopWebApp.Tests.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace OnlineShopWebApp.Tests.Areas.Views.User.Components.RolesSelection
//{
//    public class RolesSelectionViewComponentTests
//    {
//        private readonly Mock<RolesService> _rolesServiceMock;
//        private readonly RolesSelectionViewComponent _viewComponent;
//        private readonly List<RoleViewModel> _fakeRoles;

//        public RolesSelectionViewComponentTests()
//        {
//            _rolesServiceMock = new Mock<RolesService>(null!, null!, null!, null!);
//            _viewComponent = new RolesSelectionViewComponent(_rolesServiceMock.Object);

//            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
//            var mapper = config.CreateMapper();

//            _fakeRoles = FakerProvider.FakeRoles.Select(mapper.Map<RoleViewModel>)
//                                                .ToList();
//        }

//        [Fact]
//        public async Task InvokeAsync_WhenSelectedRoleExists_ReturnsViewWithSelectedRole()
//        {
//            // Arrange
//            var selectedRoleId = _fakeRoles.First().Id;

//            _rolesServiceMock.Setup(r => r.GetAllAsync())
//                             .ReturnsAsync(_fakeRoles);

//            // Act
//            var result = await _viewComponent.InvokeAsync(selectedRoleId);

//            // Assert
//            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
//            Assert.Equal("RolesSelection", viewResult.ViewName);

//            var model = Assert.IsType<(List<RoleViewModel>, Guid)>(viewResult.ViewData!.Model);
//            Assert.Equal(_fakeRoles, model.Item1);
//            Assert.Equal(selectedRoleId, model.Item2);
//        }

//        [Fact]
//        public async Task InvokeAsync_WhenSelectedRoleNotExists_ReturnsViewWithDefaultUserRole()
//        {
//            // Arrange
//            var nonExistentRoleId = Guid.NewGuid();
//            var defaultRoleId = _fakeRoles.First(r => r.Name == Constants.UserRoleName).Id;

//            _rolesServiceMock.Setup(r => r.GetAllAsync())
//                             .ReturnsAsync(_fakeRoles);

//            // Act
//            var result = await _viewComponent.InvokeAsync(nonExistentRoleId);

//            // Assert
//            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
//            Assert.Equal("RolesSelection", viewResult.ViewName);

//            var model = Assert.IsType<(List<RoleViewModel>, Guid)>(viewResult.ViewData!.Model);
//            Assert.Equal(_fakeRoles, model.Item1);
//            Assert.Equal(defaultRoleId, model.Item2);
//        }
//    }
//}
