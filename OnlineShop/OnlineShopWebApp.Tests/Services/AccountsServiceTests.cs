using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
{
    public class AccountsServiceTests
    {
        private readonly Mock<IUsersRepository> _usersRepositoryMock;
        private readonly Mock<RolesService> _rolesServiceMock;
        private readonly Mock<HashService> _hashServiceMock;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly IMapper _mapper;
        private readonly AccountsService _accountsService;

        private const int RolesCount = 10;
        private const int UsersCount = 10;
        private readonly List<Role> _fakeRoles;
        private readonly List<RoleViewModel> _fakeRolesViewModels;
        private readonly List<User> _fakeUsers;
        private readonly Faker<User> _userFaker;

        public AccountsServiceTests()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _rolesServiceMock = new Mock<RolesService>(null!, null!, null!, null!);
            _hashServiceMock = new Mock<HashService>();
            _excelServiceMock = new Mock<IExcelService>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _accountsService = new AccountsService(
                _usersRepositoryMock.Object,
                _mapper,
                _rolesServiceMock.Object,
                _hashServiceMock.Object,
                _excelServiceMock.Object);

            var _roleFaker = new Faker<Role>()
                              .RuleFor(r => r.Id, f => f.Random.Guid())
                              .RuleFor(r => r.Name, f => f.Name.JobTitle())
                              .RuleFor(r => r.CanBeDeleted, f => f.Random.Bool());

            _fakeRoles = _roleFaker.Generate(RolesCount);
            var userRole = new Role()
            {
                Id = Guid.NewGuid(),
                Name = Constants.UserRoleName,
                CanBeDeleted = false
            };

            _fakeRoles.Add(userRole);

            _fakeRolesViewModels = _fakeRoles.Select(role => _mapper.Map<RoleViewModel>(role))
                                             .ToList();

            _userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password(12))
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Role, f => f.PickRandom(_fakeRoles));

            _fakeUsers = _userFaker.Generate(UsersCount);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsersAsUserViewModel()
        {
            // Arrange
            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeUsers);

            // Act
            var result = await _accountsService.GetAllAsync();

            // Assert
            Assert.Equal(_fakeUsers.Count, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(_fakeUsers[i].Id, result[i].Id);
                Assert.Equal(_fakeUsers[i].Email, result[i].Email);
                Assert.Equal(_fakeUsers[i].Password, result[i].Password);
                Assert.Equal(_fakeUsers[i].Name, result[i].Name);
                Assert.Equal(_fakeUsers[i].Phone, result[i].Phone);
                Assert.Equal(_fakeUsers[i].Role.Id, result[i].Role.Id);
            }
        }

        [Fact]
        public async Task GetAsync_IfUserExist_ReturnsUserViewModel()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            _usersRepositoryMock.Setup(repo => repo.GetAsync(fakeUser.Id))
                                .ReturnsAsync(fakeUser);

            // Act
            var result = await _accountsService.GetAsync(fakeUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeUser.Id, result.Id);
            Assert.Equal(fakeUser.Email, result.Email);
            Assert.Equal(fakeUser.Password, result.Password);
            Assert.Equal(fakeUser.Name, result.Name);
            Assert.Equal(fakeUser.Phone, result.Phone);
            Assert.Equal(fakeUser.Role.Id, result.Role.Id);
        }

        [Fact]
        public async Task GetAsync_IfUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var fakeUser = _userFaker.Generate();
            _usersRepositoryMock.Setup(repo => repo.GetAsync(fakeUser.Id))
                                .ReturnsAsync((User)null!);

            // Act
            var result = await _accountsService.GetAsync(fakeUser.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ForUserRegistration_AddsNewUser()
        {
            // Arrange
            var userRole = _fakeRoles.First(role => role.Name == Constants.UserRoleName);
            var registerViewModel = new UserRegisterViewModel
            {
                Email = "user@example.com",
                Password = "password123",
                Name = "John Doe",
                Phone = "1234567890"
            };

            _rolesServiceMock.Setup(service => service.GetAllAsync())
                                .ReturnsAsync(_fakeRolesViewModels);

            _rolesServiceMock.Setup(service => service.GetAsync(userRole.Id))
                             .ReturnsAsync(userRole);

            _hashServiceMock.Setup(service => service.GenerateHash(registerViewModel.Password, null))
                            .Returns("hashedPassword");

            // Act
            await _accountsService.AddAsync(registerViewModel);

            // Assert
            _usersRepositoryMock.Verify(repo => repo.AddAsync(It.Is<User>(u => u.Email == registerViewModel.Email && u.Password == "hashedPassword" && u.Role == userRole)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ForAdminRegistration_AddsNewUser()
        {
            // Arrange
            var fakeRole = _fakeRoles.First();
            var registerViewModel = new AdminRegisterViewModel
            {
                Email = "user@example.com",
                Password = "password123",
                Name = "John Doe",
                Phone = "1234567890",
                RoleId = fakeRole.Id
            };

            _rolesServiceMock.Setup(service => service.GetAllAsync())
                                .ReturnsAsync(_fakeRolesViewModels);

            _rolesServiceMock.Setup(service => service.GetAsync(fakeRole.Id))
                             .ReturnsAsync(fakeRole);

            _hashServiceMock.Setup(service => service.GenerateHash(registerViewModel.Password, null))
                            .Returns("hashedPassword");

            // Act
            await _accountsService.AddAsync(registerViewModel);

            // Assert
            _usersRepositoryMock.Verify(repo => repo.AddAsync(It.Is<User>(u => u.Email == registerViewModel.Email && u.Password == "hashedPassword" && u.Role == fakeRole)), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_IfUserExists_ChangesUserPassword()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                UserId = fakeUser.Id,
                Password = "newPassword!123",
                ConfirmPassword = "newPassword!123"
            };

            _usersRepositoryMock.Setup(repo => repo.GetAsync(fakeUser.Id))
                                .ReturnsAsync(fakeUser);
            _hashServiceMock.Setup(service => service.GenerateHash(changePasswordViewModel.Password, null))
                            .Returns("newHashedPassword");

            // Act
            await _accountsService.ChangePasswordAsync(changePasswordViewModel);

            // Assert
            Assert.Equal("newHashedPassword", fakeUser.Password);
            _usersRepositoryMock.Verify(repo => repo.UpdateAsync(fakeUser), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_IfUserDoesNotExists_NotChangesUserPassword()
        {
            // Arrange
            var fakeUser = _userFaker.Generate();
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                UserId = fakeUser.Id,
                Password = "newPassword!123",
                ConfirmPassword = "newPassword!123"
            };

            _usersRepositoryMock.Setup(repo => repo.GetAsync(fakeUser.Id))
                                .ReturnsAsync((User)null!);

            // Act
            await _accountsService.ChangePasswordAsync(changePasswordViewModel);

            // Assert
            _usersRepositoryMock.Verify(repo => repo.UpdateAsync(fakeUser), Times.Never);
        }

        [Fact]
        public async Task UpdateInfoAsync_IfUserExists_UpdatesUserInfo()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var editUser = new AdminEditUserViewModel
            {
                UserId = fakeUser.Id,
                Email = "updated@example.com",
                Name = "Updated Name",
                Phone = "9876543210",
                RoleId = fakeUser.Role.Id
            };

            _usersRepositoryMock.Setup(repo => repo.GetAsync(editUser.UserId))
                                .ReturnsAsync(fakeUser);
            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleId))
                             .ReturnsAsync(fakeUser.Role);

            // Act
            await _accountsService.UpdateInfoAsync(editUser);

            // Assert
            Assert.Equal(editUser.UserId, fakeUser.Id);
            Assert.Equal(editUser.Email, fakeUser.Email);
            Assert.Equal(editUser.Name, fakeUser.Name);
            Assert.Equal(editUser.Phone, fakeUser.Phone);
            Assert.Equal(editUser.RoleId, fakeUser.Role.Id);
            _usersRepositoryMock.Verify(repo => repo.UpdateAsync(fakeUser), Times.Once);
        }

        [Fact]
        public async Task UpdateInfoAsync_IfUserDoesNotExistExists_UpdatesUserInfo()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var editUser = new AdminEditUserViewModel
            {
                UserId = Guid.NewGuid(),
                Email = "updated@example.com",
                Name = "Updated Name",
                Phone = "9876543210",
                RoleId = fakeUser.Role.Id
            };

            _usersRepositoryMock.Setup(repo => repo.GetAsync(editUser.UserId))
                                .ReturnsAsync((User)null!);
            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleId))
                             .ReturnsAsync(fakeUser.Role);

            // Act
            await _accountsService.UpdateInfoAsync(editUser);

            // Assert
            _usersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser()
        {
            // Arrange
            var userId = _fakeUsers.First().Id;

            // Act
            await _accountsService.DeleteAsync(userId);

            // Assert
            _usersRepositoryMock.Verify(repo => repo.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task IsLoginValidAsync_IfUserExistsAndPasswordMatches_ReturnsTrue()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var login = new LoginViewModel
            {
                Email = fakeUser.Email,
                Password = fakeUser.Password
            };

            _usersRepositoryMock.Setup(repo => repo.GetByEmailAsync(login.Email))
                                .ReturnsAsync(fakeUser);
            _hashServiceMock.Setup(service => service.IsEquals(login.Password, fakeUser.Password))
                            .Returns(true);

            var modelState = new ModelStateDictionary();

            // Act
            var result = await _accountsService.IsLoginValidAsync(modelState, login);

            // Assert
            Assert.True(result);
            Assert.True(modelState.IsValid);
        }

        [Fact]
        public async Task IsLoginValidAsync_IfUserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var login = new LoginViewModel
            {
                Email = "wrong@example.com",
                Password = "password123"
            };

            _usersRepositoryMock.Setup(repo => repo.GetByEmailAsync(login.Email))
                                .ReturnsAsync((User)null!);

            var modelState = new ModelStateDictionary();

            // Act
            var result = await _accountsService.IsLoginValidAsync(modelState, login);

            // Assert
            Assert.False(result);
            Assert.False(modelState.IsValid);
        }

        [Fact]
        public async Task IsLoginValidAsync_IfPasswordDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var login = new LoginViewModel
            {
                Email = fakeUser.Email,
                Password = new Faker().Internet.Password(20)
            };

            _usersRepositoryMock.Setup(repo => repo.GetByEmailAsync(login.Email))
                                .ReturnsAsync(fakeUser);
            _hashServiceMock.Setup(service => service.IsEquals(login.Password, fakeUser.Password))
                            .Returns(false);

            var modelState = new ModelStateDictionary();

            // Act
            var result = await _accountsService.IsLoginValidAsync(modelState, login);

            // Assert
            Assert.False(result);
            Assert.False(modelState.IsValid);
        }

        [Fact]
        public async Task IsRegisterValidAsync_IfEmailExists_ReturnsFalse()
        {
            // Arrange
            var register = new UserRegisterViewModel
            {
                Email = _fakeUsers.First().Email,
                Password = "password123",
                ConfirmPassword = "password123",
                Name = "User Name"
            };
            var modelState = new ModelStateDictionary();
            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeUsers);

            // Act
            var result = await _accountsService.IsRegisterValidAsync(modelState, register);

            // Assert
            Assert.False(result);
            Assert.False(modelState.IsValid);
        }

        [Fact]
        public async Task IsRegisterValidAsync_IfEmailAndPasswordMatch_ReturnsFalse()
        {
            // Arrange
            var register = new UserRegisterViewModel
            {
                Email = "valid@example.com",
                Password = "valid@example.com",
                ConfirmPassword = "password123",
                Name = "User Name"
            };
            var modelState = new ModelStateDictionary();
            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeUsers);

            // Act
            var result = await _accountsService.IsRegisterValidAsync(modelState, register);

            // Assert
            Assert.False(result);
            Assert.False(modelState.IsValid);
        }

        [Fact]
        public async Task IsRegisterValidAsync_RoleDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var registerViewModel = new AdminRegisterViewModel
            {
                Email = "valid@example.com",
                Password = "Password123!",
                Name = "Valid User",
                Phone = "1234567890",
                RoleId = Guid.NewGuid()
            };

            var modelState = new ModelStateDictionary();

            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeUsers);
            _rolesServiceMock.Setup(service => service.GetAsync(registerViewModel.RoleId))
                             .ReturnsAsync((Role)null!);

            // Act
            var result = await _accountsService.IsRegisterValidAsync(modelState, registerViewModel);

            // Assert
            Assert.False(result);
            Assert.False(modelState.IsValid);
        }

        [Fact]
        public async Task IsRegisterValidAsync_AllDataIsValid_ReturnsTrue()
        {
            // Arrange
            var fakeUser = _userFaker.Generate();
            var registerViewModel = new AdminRegisterViewModel
            {
                Email = fakeUser.Email,
                Password = fakeUser.Password,
                Name = fakeUser.Name,
                Phone = fakeUser.Phone,
                RoleId = fakeUser.Role.Id
            };

            var modelState = new ModelStateDictionary();

            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeUsers);
            _rolesServiceMock.Setup(service => service.GetAsync(registerViewModel.RoleId))
                             .ReturnsAsync(fakeUser.Role);

            // Act
            var result = await _accountsService.IsRegisterValidAsync(modelState, registerViewModel);

            // Assert
            Assert.True(result);
            Assert.True(modelState.IsValid);
        }

        [Fact]
        public async Task IsEditUserValidAsync_IfRoleDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var editUser = new AdminEditUserViewModel
            {
                UserId = fakeUser.Id,
                Email = fakeUser.Email,
                Name = fakeUser.Name,
                Phone = fakeUser.Phone,
                RoleId = Guid.NewGuid()
            };

            var modelState = new ModelStateDictionary();

            _usersRepositoryMock.Setup(repo => repo.GetAsync(editUser.UserId))
                    .ReturnsAsync(fakeUser);
            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                    .ReturnsAsync(_fakeUsers);
            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleId))
                             .ReturnsAsync((Role)null!);

            // Act
            var result = await _accountsService.IsEditUserValidAsync(modelState, editUser);

            // Assert
            Assert.False(result);
            Assert.False(modelState.IsValid);
        }

        [Fact]
        public async Task IsEditUserValidAsync_IfEmailChangedAndAlreadyExist_ReturnsFalse()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var anotherFakeUser = _fakeUsers[1];
            var editUser = new AdminEditUserViewModel
            {
                UserId = fakeUser.Id,
                Email = anotherFakeUser.Email,
                Name = fakeUser.Name,
                Phone = fakeUser.Phone,
                RoleId = Guid.NewGuid()
            };

            var modelState = new ModelStateDictionary();

            _usersRepositoryMock.Setup(repo => repo.GetAsync(editUser.UserId))
                    .ReturnsAsync(fakeUser);
            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                    .ReturnsAsync(_fakeUsers);
            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleId))
                             .ReturnsAsync((Role)null!);

            // Act
            var result = await _accountsService.IsEditUserValidAsync(modelState, editUser);

            // Assert
            Assert.False(result);
            Assert.False(modelState.IsValid);
        }

        [Fact]
        public async Task IsEditUserValidAsync_AllDataIsValid_ReturnsTrue()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var editUser = new AdminEditUserViewModel
            {
                UserId = fakeUser.Id,
                Email = fakeUser.Email,
                Name = new Faker().Name.FullName(),
                Phone = new Faker().Phone.PhoneNumber(),
                RoleId = fakeUser.Role.Id
            };

            var modelState = new ModelStateDictionary();

            _usersRepositoryMock.Setup(repo => repo.GetAsync(editUser.UserId))
                    .ReturnsAsync(fakeUser);
            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                    .ReturnsAsync(_fakeUsers);
            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleId))
                             .ReturnsAsync(fakeUser.Role);

            // Act
            var result = await _accountsService.IsEditUserValidAsync(modelState, editUser);

            // Assert
            Assert.True(result);
            Assert.True(modelState.IsValid);
        }

        [Fact]
        public async Task ChangeRolesToUserAsync_WhenCalled_ChangesRolesOfUsers()
        {
            // Arrange
            var oldRoleId = _fakeRoles.First().Id;
            var userRoleId = _fakeRoles.First(r => r.Name == Constants.UserRoleName).Id;
            _rolesServiceMock.Setup(service => service.GetAllAsync())
                             .ReturnsAsync(_fakeRolesViewModels);

            // Act
            await _accountsService.ChangeRolesToUserAsync(oldRoleId);

            // Assert
            _usersRepositoryMock.Verify(repo => repo.ChangeRolesToUserAsync(oldRoleId, userRoleId), Times.Once);
        }

        [Fact]
        public async Task ExportAllToExcelAsync_ReturnsMemoryStreamWithUsersData()
        {
            // Arrange
            var fakeStream = new MemoryStream();
            _usersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeUsers);
            _excelServiceMock.Setup(service => service.ExportUsers(It.IsAny<List<UserViewModel>>()))
                             .Returns(fakeStream);

            // Act
            var result = await _accountsService.ExportAllToExcelAsync();

            // Assert
            Assert.IsType<MemoryStream>(result);
            _usersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _excelServiceMock.Verify(service => service.ExportUsers(It.IsAny<List<UserViewModel>>()), Times.Once);
        }
    }
}
