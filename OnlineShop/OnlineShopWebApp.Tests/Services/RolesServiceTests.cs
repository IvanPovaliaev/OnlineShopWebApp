//using AutoMapper;
//using Bogus;
//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using Moq;
//using OnlineShop.Db.Models;
//using OnlineShopWebApp.Areas.Admin.Models;
//using OnlineShopWebApp.Interfaces;
//using OnlineShopWebApp.Services;
//using OnlineShopWebApp.Tests.Helpers;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace OnlineShopWebApp.Tests.Services
//{
//    public class RolesServiceTests
//    {
//        private readonly Mock<RoleManager<Role>> _rolesManagerMock;
//        private readonly Mock<IMediator> _mediatorMock;
//        private readonly Mock<IExcelService> _excelServiceMock;
//        private readonly IMapper _mapper;
//        private readonly RolesService _rolesService;

//        private readonly List<Role> _fakeRoles;
//        private readonly List<RoleViewModel> _fakeRoleViewModels;
//        private readonly Faker<Role> _roleFaker;

//        public RolesServiceTests(Mock<RoleManager<Role>> roleManagerMock, IMapper mapper, Mock<IMediator> mediatorMock, Mock<IExcelService> excelServiceMock, FakerProvider fakerProvider)
//        {
//            _rolesManagerMock = roleManagerMock;
//            _mediatorMock = mediatorMock;
//            _excelServiceMock = excelServiceMock;

//            _mapper = mapper;

//            _rolesService = new RolesService(
//                _rolesManagerMock.Object,
//                _mediatorMock.Object,
//                _mapper,
//                _excelServiceMock.Object
//            );

//            _roleFaker = fakerProvider.RoleFaker;
//            _fakeRoles = fakerProvider.FakeRoles;
//            _fakeRoleViewModels = _fakeRoles.Select(_mapper.Map<RoleViewModel>)
//                                            .ToList();
//        }

//        [Fact]
//        public async Task GetAllAsync_WhenCalled_ReturnsMappedRoles()
//        {
//            // Arrange
//            _rolesManagerMock.Setup(repo => repo.Roles)
//                .Returns(_fakeRoles.AsQueryable());

//            // Act
//            var result = await _rolesService.GetAllAsync();

//            Assert.Equal(_fakeRoles.Count, result.Count);

//            for (int i = 0; i < result.Count; i++)
//            {
//                Assert.Equal(_fakeRoles[i].Id, result[i].Id);
//                Assert.Equal(_fakeRoles[i].Name, result[i].Name);
//                Assert.Equal(_fakeRoles[i].CanBeDeleted, result[i].CanBeDeleted);
//            }

//            // Assert
//            _rolesManagerMock.Verify(repo => repo.Roles, Times.Once);
//        }

//        //[Fact]
//        //public async Task GetAsync_WhenRoleExists_ReturnRole()
//        //{
//        //    // Arrange
//        //    var expectedRole = _fakeRoles.First();

//        //    _rolesManagerMock.Setup(repo => repo.GetAsync(expectedRole.Id))
//        //                        .ReturnsAsync(expectedRole);

//        //    // Act
//        //    var result = await _rolesService.GetAsync(expectedRole.Id);

//        //    // Assert
//        //    Assert.NotNull(result);
//        //    Assert.Equal(expectedRole.Id, result.Id);
//        //    Assert.Equal(expectedRole.Name, result.Name);
//        //    Assert.Equal(expectedRole.CanBeDeleted, result.CanBeDeleted);
//        //    _rolesManagerMock.Verify(repo => repo.GetAsync(expectedRole.Id), Times.Once);
//        //}

//        //[Fact]
//        //public async Task GetAsync_WhenRoleDoesNotExist_ReturnNull()
//        //{
//        //    // Arrange
//        //    var roleId = Guid.NewGuid();

//        //    _rolesManagerMock.Setup(repo => repo.GetAsync(roleId))
//        //                        .ReturnsAsync((Role)null!);

//        //    // Act
//        //    var result = await _rolesService.GetAsync(roleId);

//        //    // Assert
//        //    Assert.Null(result);
//        //    _rolesManagerMock.Verify(repo => repo.GetAsync(roleId), Times.Once);
//        //}

//        //[Fact]
//        //public async Task GetViewModelAsync_WhenRoleExist_ReturnMappedRole()
//        //{
//        //    // Arrange
//        //    var fakeRole = _roleFaker.Generate();
//        //    var expectedRoleViewModel = new RoleViewModel
//        //    {
//        //        Id = fakeRole.Id,
//        //        Name = fakeRole.Name,
//        //        CanBeDeleted = fakeRole.CanBeDeleted
//        //    };

//        //    _rolesManagerMock.Setup(repo => repo.GetAsync(fakeRole.Id))
//        //                        .ReturnsAsync(fakeRole);

//        //    // Act
//        //    var result = await _rolesService.GetViewModelAsync(fakeRole.Id);

//        //    // Assert
//        //    Assert.NotNull(result);
//        //    Assert.Equal(expectedRoleViewModel.Name, result.Name);
//        //    Assert.Equal(expectedRoleViewModel.Id, result.Id);
//        //    Assert.Equal(expectedRoleViewModel.CanBeDeleted, result.CanBeDeleted);
//        //    _rolesManagerMock.Verify(repo => repo.GetAsync(fakeRole.Id), Times.Once);
//        //}

//        //[Fact]
//        //public async Task IsNewValidAsync_IfRoleWithSameNameExists_ReturnFalse()
//        //{
//        //    // Arrange
//        //    var fakeRole = _fakeRoleViewModels.First();
//        //    var existingRoles = new List<RoleViewModel>
//        //    {
//        //        new () { Name = fakeRole.Name }
//        //    };

//        //    var modelState = new ModelStateDictionary();

//        //    _rolesManagerMock.Setup(repo => repo.GetAllAsync())
//        //                        .ReturnsAsync(existingRoles.Select(r => new Role { Name = r.Name })
//        //                                                   .ToList());

//        //    // Act
//        //    var result = await _rolesService.IsNewValidAsync(modelState, fakeRole);

//        //    // Assert
//        //    Assert.False(result);
//        //    Assert.Contains(modelState, m => m.Value!.Errors.Any());
//        //    _rolesManagerMock.Verify(repo => repo.GetAllAsync(), Times.Once);
//        //}

//        //[Fact]
//        //public async Task IsNewValidAsync_IfRoleWithSameNameNotExists_ShouldReturnTrue_()
//        //{
//        //    // Arrange
//        //    var fakeRole = _fakeRoleViewModels.First();
//        //    var existingRoles = new List<RoleViewModel>
//        //    {
//        //        new () { Name = $"{fakeRole.Name}-copy" }
//        //    };

//        //    var modelState = new ModelStateDictionary();

//        //    _rolesManagerMock.Setup(repo => repo.GetAllAsync())
//        //                        .ReturnsAsync(existingRoles.Select(r => new Role { Name = r.Name })
//        //                                                   .ToList());

//        //    // Act
//        //    var result = await _rolesService.IsNewValidAsync(modelState, fakeRole);

//        //    // Assert
//        //    Assert.True(result);
//        //    Assert.Equal(0, modelState.ErrorCount);
//        //    _rolesManagerMock.Verify(repo => repo.GetAllAsync(), Times.Once);
//        //}

//        //[Fact]
//        //public async Task AddAsync_WithGivenMappedRole_ShouldAddMappedRoleToRepository()
//        //{
//        //    // Arrange
//        //    var roleViewModel = _fakeRoleViewModels.First();
//        //    var role = _fakeRoles.First();

//        //    // Act
//        //    await _rolesService.AddAsync(roleViewModel);

//        //    // Assert
//        //    _rolesManagerMock.Verify(repo => repo.AddAsync(It.Is<Role>(r => r.Name == role.Name)), Times.Once);
//        //}

//        //[Fact]
//        //public async Task DeleteAsync_IfCanBeDeleted_ShouldDeleteRole()
//        //{
//        //    // Arrange
//        //    var role = _roleFaker.Generate();

//        //    var deletableRole = new Role
//        //    {
//        //        Id = role.Id,
//        //        Name = role.Name,
//        //        CanBeDeleted = true
//        //    };

//        //    _rolesManagerMock.Setup(repo => repo.GetAsync(deletableRole.Id))
//        //                        .ReturnsAsync(deletableRole);
//        //    _rolesManagerMock.Setup(repo => repo.DeleteAsync(deletableRole.Id))
//        //                        .Returns(Task.CompletedTask);

//        //    // Act
//        //    await _rolesService.DeleteAsync(deletableRole.Id);

//        //    // Assert
//        //    _mediatorMock.Verify(mediator => mediator.Publish(It.Is<RoleDeletedNotification>(n => n.RoleId == deletableRole.Id), default), Times.Once);
//        //    _rolesManagerMock.Verify(repo => repo.DeleteAsync(deletableRole.Id), Times.Once);
//        //}

//        //[Fact]
//        //public async Task DeleteAsync_IfCanNotBeDeleted_ShouldNotDeleteRole()
//        //{
//        //    // Arrange
//        //    var role = _roleFaker.Generate();

//        //    var deletableRole = new Role
//        //    {
//        //        Id = role.Id,
//        //        Name = role.Name,
//        //        CanBeDeleted = false
//        //    };

//        //    _rolesManagerMock.Setup(repo => repo.GetAsync(deletableRole.Id))
//        //                        .ReturnsAsync(deletableRole);
//        //    _rolesManagerMock.Setup(repo => repo.DeleteAsync(deletableRole.Id))
//        //                        .Returns(Task.CompletedTask);

//        //    // Act
//        //    await _rolesService.DeleteAsync(deletableRole.Id);

//        //    // Assert
//        //    _mediatorMock.Verify(mediator => mediator.Publish(It.Is<RoleDeletedNotification>(n => n.RoleId == deletableRole.Id), default), Times.Never);
//        //    _rolesManagerMock.Verify(repo => repo.DeleteAsync(deletableRole.Id), Times.Never);
//        //}

//        //[Fact]
//        //public async Task ExportAllToExcelAsync_WhenCalled_ReturnsMemoryStream()
//        //{
//        //    // Arrange
//        //    var memoryStream = new MemoryStream();

//        //    _rolesManagerMock.Setup(repo => repo.GetAllAsync())
//        //                        .ReturnsAsync(_fakeRoles);

//        //    _excelServiceMock.Setup(service => service.ExportRoles(It.IsAny<List<RoleViewModel>>()))
//        //                     .Returns(memoryStream);

//        //    // Act
//        //    var result = await _rolesService.ExportAllToExcelAsync();

//        //    // Assert
//        //    Assert.IsType<MemoryStream>(result);
//        //    _rolesManagerMock.Verify(repo => repo.GetAllAsync(), Times.Once);
//        //    _excelServiceMock.Verify(service => service.ExportRoles(It.IsAny<List<RoleViewModel>>()), Times.Once);
//        //}
//    }
//}

