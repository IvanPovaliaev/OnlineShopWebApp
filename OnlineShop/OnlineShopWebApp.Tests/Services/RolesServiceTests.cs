using AutoMapper;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
{
    public class RolesServiceTests
    {
        private readonly Mock<IRolesRepository> _rolesRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RolesService _rolesService;

        private readonly List<Role> _fakeRoles;
        private readonly List<RoleViewModel> _fakeRoleViewModels;
        private readonly Faker<Role> _roleFaker;
        private const int RolesCount = 10;

        public RolesServiceTests()
        {
            _rolesRepositoryMock = new Mock<IRolesRepository>();
            _mediatorMock = new Mock<IMediator>();
            _excelServiceMock = new Mock<IExcelService>();
            _mapperMock = new Mock<IMapper>();

            _rolesService = new RolesService(
                _rolesRepositoryMock.Object,
                _mediatorMock.Object,
                _mapperMock.Object,
                _excelServiceMock.Object
            );

            _roleFaker = new Faker<Role>().RuleFor(r => r.Id, f => f.Random.Guid())
                                          .RuleFor(r => r.Name, f => f.Name.JobTitle())
                                          .RuleFor(r => r.CanBeDeleted, f => f.Random.Bool());

            _fakeRoles = _roleFaker.Generate(RolesCount);
            _fakeRoleViewModels = _fakeRoles.Select(r => new RoleViewModel { Id = r.Id, Name = r.Name, CanBeDeleted = r.CanBeDeleted })
                                            .ToList();

            _mapperMock.Setup(mapper => mapper.Map<RoleViewModel>(It.IsAny<Role>()))
                       .Returns((Role role) => new RoleViewModel
                       {
                           Id = role.Id,
                           Name = role.Name,
                           CanBeDeleted = role.CanBeDeleted
                       });
        }

        [Fact]
        public async Task GetAllAsync_WhenCallde_ReturnsMappedRoles()
        {
            _rolesRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeRoles);

            var result = await _rolesService.GetAllAsync();

            Assert.Equal(_fakeRoles.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(_fakeRoles[i].Id, result[i].Id);
                Assert.Equal(_fakeRoles[i].Name, result[i].Name);
                Assert.Equal(_fakeRoles[i].CanBeDeleted, result[i].CanBeDeleted);
            }

            _rolesRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenRoleExists_ReturnRole()
        {
            var expectedRole = _fakeRoles.First();

            _rolesRepositoryMock.Setup(repo => repo.GetAsync(expectedRole.Id))
                                .ReturnsAsync(expectedRole);

            var result = await _rolesService.GetAsync(expectedRole.Id);

            Assert.NotNull(result);
            Assert.Equal(expectedRole.Id, result.Id);
            Assert.Equal(expectedRole.Name, result.Name);
            Assert.Equal(expectedRole.CanBeDeleted, result.CanBeDeleted);
            _rolesRepositoryMock.Verify(repo => repo.GetAsync(expectedRole.Id), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenRoleDoesNotExist_ShouldReturnNull()
        {
            var roleId = Guid.NewGuid();

            _rolesRepositoryMock.Setup(repo => repo.GetAsync(roleId))
                                .ReturnsAsync((Role)null!);

            var result = await _rolesService.GetAsync(roleId);

            Assert.Null(result);
            _rolesRepositoryMock.Verify(repo => repo.GetAsync(roleId), Times.Once);
        }

        [Fact]
        public async Task GetViewModelAsync_WhenRoleExist_ShouldReturnMappedRole()
        {
            var fakeRole = _roleFaker.Generate();
            var expectedRoleViewModel = new RoleViewModel
            {
                Id = fakeRole.Id,
                Name = fakeRole.Name,
                CanBeDeleted = fakeRole.CanBeDeleted
            };

            _rolesRepositoryMock.Setup(repo => repo.GetAsync(fakeRole.Id))
                                .ReturnsAsync(fakeRole);
            _mapperMock.Setup(mapper => mapper.Map<RoleViewModel>(fakeRole))
                                              .Returns(expectedRoleViewModel);

            var result = await _rolesService.GetViewModelAsync(fakeRole.Id);

            Assert.NotNull(result);
            Assert.Equal(expectedRoleViewModel.Name, result.Name);
            Assert.Equal(expectedRoleViewModel.Id, result.Id);
            Assert.Equal(expectedRoleViewModel.CanBeDeleted, result.CanBeDeleted);
            _rolesRepositoryMock.Verify(repo => repo.GetAsync(fakeRole.Id), Times.Once);
        }

        [Fact]
        public async Task IsNewValidAsync_IfRoleWithSameNameExists_ReturnFalse_()
        {
            var fakeRole = _fakeRoleViewModels.First();
            var existingRoles = new List<RoleViewModel>
            {
                new () { Name = fakeRole.Name }
            };

            var modelState = new ModelStateDictionary();

            _rolesRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(existingRoles.Select(r => new Role { Name = r.Name })
                                                           .ToList());

            var result = await _rolesService.IsNewValidAsync(modelState, fakeRole);

            Assert.False(result);
            Assert.Contains(modelState, m => m.Value!.Errors.Any());
            _rolesRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task IsNewValidAsync_IfRoleWithSameNameNotExists_ShouldReturnTrue_()
        {
            var fakeRole = _fakeRoleViewModels.First();
            var existingRoles = new List<RoleViewModel>
            {
                new () { Name = $"{fakeRole.Name}-copy" }
            };

            var modelState = new ModelStateDictionary();

            _rolesRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(existingRoles.Select(r => new Role { Name = r.Name })
                                                           .ToList());

            var result = await _rolesService.IsNewValidAsync(modelState, fakeRole);

            Assert.True(result);
            Assert.Equal(0, modelState.ErrorCount);
            _rolesRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithGivenMappedRole_ShouldAddMappedRoleToRepository()
        {
            var roleViewModel = _fakeRoleViewModels.First();
            var role = _fakeRoles.First();

            _mapperMock.Setup(mapper => mapper.Map<Role>(roleViewModel))
                       .Returns(role);

            await _rolesService.AddAsync(roleViewModel);

            _rolesRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Role>(r => r.Name == role.Name)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_IfCanBeDeleted_ShouldDeleteRole()
        {
            var role = _roleFaker.Generate();

            var deletableRole = new Role
            {
                Id = role.Id,
                Name = role.Name,
                CanBeDeleted = true
            };

            _rolesRepositoryMock.Setup(repo => repo.GetAsync(deletableRole.Id))
                                .ReturnsAsync(deletableRole);
            _rolesRepositoryMock.Setup(repo => repo.DeleteAsync(deletableRole.Id))
                                .Returns(Task.CompletedTask);

            await _rolesService.DeleteAsync(deletableRole.Id);

            _mediatorMock.Verify(mediator => mediator.Publish(It.Is<RoleDeletedNotification>(n => n.RoleId == deletableRole.Id), default), Times.Once);
            _rolesRepositoryMock.Verify(repo => repo.DeleteAsync(deletableRole.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_IfCanNotBeDeleted_ShouldNotDeleteRole()
        {
            var role = _roleFaker.Generate();

            var deletableRole = new Role
            {
                Id = role.Id,
                Name = role.Name,
                CanBeDeleted = false
            };

            _rolesRepositoryMock.Setup(repo => repo.GetAsync(deletableRole.Id))
                                .ReturnsAsync(deletableRole);
            _rolesRepositoryMock.Setup(repo => repo.DeleteAsync(deletableRole.Id))
                                .Returns(Task.CompletedTask);

            await _rolesService.DeleteAsync(deletableRole.Id);

            _mediatorMock.Verify(mediator => mediator.Publish(It.Is<RoleDeletedNotification>(n => n.RoleId == deletableRole.Id), default), Times.Never);
            _rolesRepositoryMock.Verify(repo => repo.DeleteAsync(deletableRole.Id), Times.Never);
        }

        [Fact]
        public async Task ExportAllToExcelAsync_WhenCalled_ReturnMemoryStream()
        {
            var memoryStream = new MemoryStream();

            _rolesRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeRoles);

            _excelServiceMock.Setup(service => service.ExportRoles(It.IsAny<List<RoleViewModel>>()))
                             .Returns(memoryStream);

            var result = await _rolesService.ExportAllToExcelAsync();

            Assert.IsType<MemoryStream>(result);
        }
    }
}

