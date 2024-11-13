using AutoMapper;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
{
    public class RolesServiceTests
    {
        private readonly Mock<RoleManager<Role>> _rolesManagerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly IMapper _mapper;
        private readonly RolesService _rolesService;

        private readonly List<Role> _fakeRoles;
        private readonly List<RoleViewModel> _fakeRoleViewModels;
        private readonly Faker<Role> _roleFaker;

        public RolesServiceTests(IMapper mapper, Mock<IMediator> mediatorMock, Mock<IExcelService> excelServiceMock, FakerProvider fakerProvider)
        {
            _rolesManagerMock = new Mock<RoleManager<Role>>(
                new Mock<IRoleStore<Role>>().Object,
                null!,
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<Role>>>().Object);

            _mediatorMock = mediatorMock;
            _excelServiceMock = excelServiceMock;
            _mapper = mapper;

            _rolesService = new RolesService(
                _rolesManagerMock.Object,
                _mediatorMock.Object,
                _mapper,
                _excelServiceMock.Object
            );

            _roleFaker = fakerProvider.RoleFaker;
            _fakeRoles = fakerProvider.FakeRoles;
            _fakeRoleViewModels = _fakeRoles.Select(_mapper.Map<RoleViewModel>)
                                            .ToList();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsMappedRoles()
        {
            // Arrange
            _rolesManagerMock.Setup(rm => rm.Roles)
                             .Returns(new TestAsyncEnumerable<Role>(_fakeRoles).AsQueryable());

            // Act
            var result = await _rolesService.GetAllAsync();

            Assert.Equal(_fakeRoles.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(_fakeRoles[i].Id, result[i].Id);
                Assert.Equal(_fakeRoles[i].Name, result[i].Name);
                Assert.Equal(_fakeRoles[i].CanBeDeleted, result[i].CanBeDeleted);
            }

            // Assert
            _rolesManagerMock.Verify(repo => repo.Roles, Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenRoleExists_ReturnRole()
        {
            // Arrange
            var expectedRole = _fakeRoles.First();

            _rolesManagerMock.Setup(repo => repo.FindByNameAsync(expectedRole.Name!))
                                .ReturnsAsync(expectedRole);

            // Act
            var result = await _rolesService.GetAsync(expectedRole.Name!);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRole.Id, result.Id);
            Assert.Equal(expectedRole.Name, result.Name);
            Assert.Equal(expectedRole.CanBeDeleted, result.CanBeDeleted);
            _rolesManagerMock.Verify(repo => repo.FindByNameAsync(expectedRole.Name!), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenRoleDoesNotExist_ReturnNull()
        {
            // Arrange
            var roleName = _roleFaker.Generate().Name;

            _rolesManagerMock.Setup(repo => repo.FindByNameAsync(roleName!))
                                .ReturnsAsync((Role)null!);

            // Act
            var result = await _rolesService.GetAsync(roleName!);

            // Assert
            Assert.Null(result);
            _rolesManagerMock.Verify(repo => repo.FindByNameAsync(roleName), Times.Once);
        }

        [Fact]
        public async Task IsNewValidAsync_IfRoleWithSameNameExists_ReturnFalse()
        {
            // Arrange
            var fakeRole = new AddRoleViewModel()
            {
                Name = _fakeRoleViewModels.First().Name
            };

            var existingRoles = new List<Role>
            {
                new () { Name = fakeRole.Name }
            };

            var modelState = new ModelStateDictionary();

            _rolesManagerMock.Setup(rm => rm.Roles)
                             .Returns(new TestAsyncEnumerable<Role>(existingRoles).AsQueryable());

            // Act
            var result = await _rolesService.IsNewValidAsync(modelState, fakeRole);

            // Assert
            Assert.False(result);
            Assert.Contains(modelState, m => m.Value!.Errors.Any());
            _rolesManagerMock.Verify(repo => repo.Roles, Times.Once);
        }

        [Fact]
        public async Task IsNewValidAsync_IfRoleWithSameNameNotExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeRole = new AddRoleViewModel()
            {
                Name = _fakeRoleViewModels.First().Name
            };

            var existingRoles = new List<Role>
            {
                new () { Name = $"{fakeRole.Name}-copy" }
            };

            var modelState = new ModelStateDictionary();

            _rolesManagerMock.Setup(rm => rm.Roles)
                             .Returns(new TestAsyncEnumerable<Role>(existingRoles).AsQueryable());

            // Act
            var result = await _rolesService.IsNewValidAsync(modelState, fakeRole);

            // Assert
            Assert.True(result);
            Assert.Equal(0, modelState.ErrorCount);
            _rolesManagerMock.Verify(repo => repo.Roles, Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithGivenAddRole_ShouldAddRoleToRepository()
        {
            // Arrange
            var fakeRole = new AddRoleViewModel()
            {
                Name = _fakeRoleViewModels.First().Name
            };

            var role = _fakeRoles.First();

            // Act
            await _rolesService.AddAsync(fakeRole);

            // Assert
            _rolesManagerMock.Verify(repo => repo.CreateAsync(It.Is<Role>(r => r.Name == role.Name)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_IfCanBeDeleted_ShouldDeleteRole()
        {
            // Arrange
            var role = _roleFaker.Generate();

            var deletableRole = new Role
            {
                Id = role.Id,
                Name = role.Name,
                CanBeDeleted = true
            };

            _rolesManagerMock.Setup(repo => repo.FindByNameAsync(deletableRole.Name!))
                                .ReturnsAsync(deletableRole);
            _rolesManagerMock.Setup(repo => repo.DeleteAsync(deletableRole))
                                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _rolesService.DeleteAsync(deletableRole.Name);

            // Assert
            _mediatorMock.Verify(mediator => mediator.Publish(It.Is<RoleDeletedNotification>(n => n.RoleName == deletableRole.Name), default), Times.Once);
            _rolesManagerMock.Verify(repo => repo.DeleteAsync(deletableRole), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_IfCanNotBeDeleted_ShouldNotDeleteRole()
        {
            // Arrange
            var role = _roleFaker.Generate();

            var deletableRole = new Role
            {
                Id = role.Id,
                Name = role.Name,
                CanBeDeleted = false
            };

            _rolesManagerMock.Setup(repo => repo.FindByNameAsync(deletableRole.Name!))
                                .ReturnsAsync(deletableRole);
            _rolesManagerMock.Setup(repo => repo.DeleteAsync(deletableRole))
                                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _rolesService.DeleteAsync(deletableRole.Name!);

            // Assert
            _mediatorMock.Verify(mediator => mediator.Publish(It.Is<RoleDeletedNotification>(n => n.RoleName == deletableRole.Name), default), Times.Never);
            _rolesManagerMock.Verify(repo => repo.DeleteAsync(deletableRole), Times.Never);
        }

        [Fact]
        public async Task ExportAllToExcelAsync_WhenCalled_ReturnsMemoryStream()
        {
            // Arrange
            var memoryStream = new MemoryStream();

            _rolesManagerMock.Setup(rm => rm.Roles)
                             .Returns(new TestAsyncEnumerable<Role>(_fakeRoles).AsQueryable());

            _excelServiceMock.Setup(service => service.ExportRoles(It.IsAny<List<RoleViewModel>>()))
                             .Returns(memoryStream);

            // Act
            var result = await _rolesService.ExportAllToExcelAsync();

            // Assert
            Assert.IsType<MemoryStream>(result);
            _rolesManagerMock.Verify(repo => repo.Roles, Times.Once);
            _excelServiceMock.Verify(service => service.ExportRoles(It.IsAny<List<RoleViewModel>>()), Times.Once);
        }
    }
}

