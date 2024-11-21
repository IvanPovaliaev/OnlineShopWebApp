using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Moq;
using OnlineShop.Db;
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
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IRolesService> _rolesServiceMock;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly IMapper _mapper;
        private readonly AccountsService _accountsService;

        private readonly List<Role> _fakeRoles;
        private readonly List<User> _fakeUsers;
        private readonly Faker<User> _userFaker;

        public AccountsServiceTests(Mock<IRolesService> rolesServiceMock, IMapper mapper, Mock<IExcelService> excelServiceMock, FakerProvider fakerProvider, Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            _mapper = mapper;
            _rolesServiceMock = rolesServiceMock;
            _excelServiceMock = excelServiceMock;

            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                httpContextAccessorMock.Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null!,
                null!,
                null!,
                null!
                );

            var configurationMock = new Mock<IConfiguration>();
            var imageProviderMock = new Mock<ImagesProvider>(null!);

            _accountsService = new AccountsService(
                _mapper,
                _rolesServiceMock.Object,
                _excelServiceMock.Object,
                _signInManagerMock.Object,
                _userManagerMock.Object,
                configurationMock.Object,
                imageProviderMock.Object);

            _fakeRoles = fakerProvider.FakeRoles;

            _userFaker = fakerProvider.UserFaker;
            _fakeUsers = fakerProvider.FakeUsers;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsersAsUserViewModel()
        {
            // Arrange
            var fakeRolesNames = _fakeRoles.Select(r => r.Name)
                                           .ToList();

            _userManagerMock.Setup(um => um.Users)
                             .Returns(new TestAsyncEnumerable<User>(_fakeUsers).AsQueryable());

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                            .ReturnsAsync(fakeRolesNames!);

            // Act
            var result = await _accountsService.GetAllAsync();

            // Assert
            Assert.Equal(_fakeUsers.Count, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(_fakeUsers[i].Id, result[i].Id);
                Assert.Equal(_fakeUsers[i].Email, result[i].Email);
                Assert.Equal(_fakeUsers[i].FullName, result[i].FullName);
                Assert.Equal(_fakeUsers[i].PhoneNumber, result[i].PhoneNumber);
            }
        }

        [Fact]
        public async Task GetAsync_IfUserExist_ReturnsUserViewModel()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var fakeRolesNames = _fakeRoles.Select(r => r.Name)
                                           .ToList();

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUser.Id))
                            .ReturnsAsync(fakeUser);

            _userManagerMock.Setup(um => um.GetRolesAsync(fakeUser))
                            .ReturnsAsync(fakeRolesNames!);

            // Act
            var result = await _accountsService.GetAsync(fakeUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeUser.Id, result.Id);
            Assert.Equal(fakeUser.Email, result.Email);
            Assert.Equal(fakeUser.FullName, result.FullName);
            Assert.Equal(fakeUser.PhoneNumber, result.PhoneNumber);
            Assert.Equal(fakeRolesNames.FirstOrDefault(), result.RoleName);
        }

        [Fact]
        public async Task GetAsync_IfUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var fakeUserId = Guid.NewGuid().ToString();

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUserId))
                            .ReturnsAsync((User)null!);

            // Act
            var result = await _accountsService.GetAsync(fakeUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEditViewModelAsync_IfUserExist_ReturnsEditUserViewModel()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUser.Id))
                            .ReturnsAsync(fakeUser);

            // Act
            var result = await _accountsService.GetEditViewModelAsync(fakeUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeUser.Id, result.Id);
            Assert.Equal(fakeUser.Email, result.Email);
            Assert.Equal(fakeUser.FullName, result.FullName);
            Assert.Equal(fakeUser.PhoneNumber, result.PhoneNumber);
            Assert.Equal(fakeUser.AvatarUrl, result.AvatarUrl);
        }

        [Fact]
        public async Task GetEditViewModelAsync_IfUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var fakeUserId = Guid.NewGuid().ToString();

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUserId))
                            .ReturnsAsync((User)null!);

            // Act
            var result = await _accountsService.GetAsync(fakeUserId);

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
                PhoneNumber = "1234567890"
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.Is<User>(user => user.Email == registerViewModel.Email), registerViewModel.Password))
                            .ReturnsAsync(IdentityResult.Success);

            _rolesServiceMock.Setup(service => service.GetAsync(userRole.Name!))
                             .ReturnsAsync(userRole);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.Is<User>(user => user.Email == registerViewModel.Email), userRole.Name!))
                            .ReturnsAsync(IdentityResult.Success);

            _signInManagerMock.Setup(sm => sm.SignInAsync(It.Is<User>(user => user.Email == registerViewModel.Email), false, null!))
                              .Returns(Task.CompletedTask);

            // Act
            await _accountsService.AddAsync(registerViewModel);

            // Assert
            _userManagerMock.Verify(um => um.CreateAsync(It.Is<User>(user => user.Email == registerViewModel.Email), registerViewModel.Password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.Is<User>(user => user.Email == registerViewModel.Email), userRole.Name!), Times.Once);
            _signInManagerMock.Verify(sm => sm.SignInAsync(It.Is<User>(user => user.Email == registerViewModel.Email), false, null!), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ForAdminRegistration_AddsNewUser()
        {
            var fakerRole = _fakeRoles.First();
            var registerViewModel = new AdminRegisterViewModel
            {
                Email = "user@example.com",
                Password = "password123",
                Name = "John Doe",
                PhoneNumber = "1234567890",
                RoleName = fakerRole.Name!
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.Is<User>(user => user.Email == registerViewModel.Email), registerViewModel.Password))
                                        .ReturnsAsync(IdentityResult.Success);

            _rolesServiceMock.Setup(service => service.GetAsync(registerViewModel.RoleName))
                             .ReturnsAsync(fakerRole);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.Is<User>(user => user.Email == registerViewModel.Email), registerViewModel.RoleName))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            await _accountsService.AddAsync(registerViewModel);

            // Assert
            _userManagerMock.Verify(um => um.CreateAsync(It.Is<User>(user => user.Email == registerViewModel.Email), registerViewModel.Password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.Is<User>(user => user.Email == registerViewModel.Email), registerViewModel.RoleName), Times.Once);
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

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUser.Id))
                            .ReturnsAsync(fakeUser);

            var passwordHasherMock = new Mock<IPasswordHasher<User>>();

            passwordHasherMock.Setup(ph => ph.HashPassword(fakeUser, changePasswordViewModel.Password))
                              .Returns("mockedPasswordHash");

            _userManagerMock.Object.PasswordHasher = passwordHasherMock.Object;

            // Act
            await _accountsService.ChangePasswordAsync(changePasswordViewModel);

            // Assert
            Assert.Equal("mockedPasswordHash", fakeUser.PasswordHash);
            _userManagerMock.Verify(um => um.UpdateAsync(fakeUser), Times.Once);
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

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUser.Id))
                            .ReturnsAsync(((User)null!));

            // Act
            await _accountsService.ChangePasswordAsync(changePasswordViewModel);

            // Assert
            _userManagerMock.Verify(um => um.UpdateAsync(fakeUser), Times.Never);
        }

        [Fact]
        public async Task UpdateInfoAsync_IfUserExists_UpdatesUserInfo()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var fakeRole = _fakeRoles.First();
            var fakeRoleNames = _fakeRoles.Select(r => r.Name);
            var editUser = new AdminEditUserViewModel
            {
                Id = fakeUser.Id,
                Email = "updated@example.com",
                FullName = "Updated Name",
                PhoneNumber = "9876543210",
                RoleName = fakeRole.Name
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUser.Id))
                            .ReturnsAsync(fakeUser);

            _userManagerMock.Setup(um => um.UpdateAsync(fakeUser))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(fakeUser, editUser.RoleName!))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.RemoveFromRolesAsync(fakeUser, fakeRoleNames!))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            await _accountsService.UpdateInfoAsync(editUser);

            // Assert
            Assert.Equal(editUser.Id, fakeUser.Id);
            Assert.Equal(editUser.Email, fakeUser.Email);
            Assert.Equal(editUser.FullName, fakeUser.FullName);
            Assert.Equal(editUser.PhoneNumber, fakeUser.PhoneNumber);
            _userManagerMock.Verify(repo => repo.UpdateAsync(fakeUser), Times.Once);
        }

        [Fact]
        public async Task UpdateInfoAsync_IfUserDoesNotExistExists_UpdatesUserInfo()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var fakeRole = _fakeRoles.First();
            var editUser = new AdminEditUserViewModel
            {
                Id = Guid.NewGuid().ToString(),
                Email = "updated@example.com",
                FullName = "Updated Name",
                PhoneNumber = "9876543210",
                RoleName = fakeRole.Name
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUser.Id))
                .ReturnsAsync((User)null!);

            // Act
            await _accountsService.UpdateInfoAsync(editUser);

            // Assert
            _userManagerMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();

            _userManagerMock.Setup(um => um.FindByIdAsync(fakeUser.Id))
                .ReturnsAsync(fakeUser);

            // Act
            await _accountsService.DeleteAsync(fakeUser.Id);

            // Assert
            _userManagerMock.Verify(repo => repo.DeleteAsync(fakeUser), Times.Once);
        }

        [Fact]
        public async Task IsLoginValidAsync_IfUserExistsAndPasswordMatches_ReturnsTrue()
        {
            // Arrange
            var fakeUser = _fakeUsers.First();
            var login = new LoginViewModel
            {
                Email = fakeUser.Email,
                Password = fakeUser.PasswordHash,
                KeepMeLogged = true
            };

            _signInManagerMock.Setup(repo => repo.PasswordSignInAsync(login.Email, login.Password, login.KeepMeLogged, false))
                              .ReturnsAsync(SignInResult.Success);

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
            var fakeUser = _fakeUsers.First();
            var login = new LoginViewModel
            {
                Email = fakeUser.Email,
                Password = fakeUser.PasswordHash,
                KeepMeLogged = true
            };

            _signInManagerMock.Setup(repo => repo.PasswordSignInAsync(login.Email, login.Password, login.KeepMeLogged, false))
                              .ReturnsAsync(SignInResult.Failed);

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
            _userManagerMock.Setup(um => um.Users)
                            .Returns(new TestAsyncEnumerable<User>(_fakeUsers).AsQueryable());

            _userManagerMock.Setup(um => um.FindByEmailAsync(register.Email))
                            .ReturnsAsync(_fakeUsers.First());

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
            _userManagerMock.Setup(um => um.Users)
                            .Returns(new TestAsyncEnumerable<User>(_fakeUsers).AsQueryable());

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
                PhoneNumber = "1234567890",
                RoleName = $"{_fakeRoles.First().Name}-copy"
            };

            var modelState = new ModelStateDictionary();

            _userManagerMock.Setup(um => um.Users)
                            .Returns(new TestAsyncEnumerable<User>(_fakeUsers).AsQueryable());

            _rolesServiceMock.Setup(service => service.GetAsync(registerViewModel.RoleName))
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
                Password = fakeUser.PasswordHash,
                Name = fakeUser.FullName,
                PhoneNumber = fakeUser.PhoneNumber,
                RoleName = _fakeRoles.First().Name
            };

            var modelState = new ModelStateDictionary();

            _userManagerMock.Setup(um => um.Users)
                            .Returns(new TestAsyncEnumerable<User>(_fakeUsers).AsQueryable());

            _rolesServiceMock.Setup(service => service.GetAsync(registerViewModel.RoleName))
                             .ReturnsAsync(_fakeRoles.First());

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
                Id = fakeUser.Id,
                Email = fakeUser.Email!,
                FullName = fakeUser.FullName,
                PhoneNumber = fakeUser.PhoneNumber,
                RoleName = _fakeRoles.First().Name!
            };

            var modelState = new ModelStateDictionary();

            var fakeRolesNames = _fakeRoles.Select(r => r.Name)
                               .ToList();

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                            .ReturnsAsync(fakeRolesNames!);

            _userManagerMock.Setup(repo => repo.FindByIdAsync(editUser.Id))
                            .ReturnsAsync(fakeUser);

            _userManagerMock.Setup(repo => repo.FindByEmailAsync(editUser.Email))
                            .ReturnsAsync((User)null!);

            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleName))
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
                Id = fakeUser.Id,
                Email = anotherFakeUser.Email!,
                FullName = fakeUser.FullName,
                PhoneNumber = fakeUser.PhoneNumber,
                RoleName = _fakeRoles.First().Name!
            };

            var modelState = new ModelStateDictionary();

            var fakeRolesNames = _fakeRoles.Select(r => r.Name)
                   .ToList();

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                            .ReturnsAsync(fakeRolesNames!);

            _userManagerMock.Setup(repo => repo.FindByIdAsync(editUser.Id))
                            .ReturnsAsync(fakeUser);

            _userManagerMock.Setup(repo => repo.FindByEmailAsync(editUser.Email))
                            .ReturnsAsync(anotherFakeUser);

            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleName))
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
                Id = fakeUser.Id,
                Email = fakeUser.Email!,
                FullName = new Faker().Name.FullName(),
                PhoneNumber = new Faker().Phone.PhoneNumber(),
                RoleName = _fakeRoles.First().Name!
            };

            var modelState = new ModelStateDictionary();

            var fakeRolesNames = _fakeRoles.Select(r => r.Name)
                   .ToList();

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                            .ReturnsAsync(fakeRolesNames!);

            _userManagerMock.Setup(repo => repo.FindByIdAsync(editUser.Id))
                            .ReturnsAsync(fakeUser);

            _userManagerMock.Setup(repo => repo.FindByEmailAsync(editUser.Email))
                            .ReturnsAsync((User)null!);

            _rolesServiceMock.Setup(service => service.GetAsync(editUser.RoleName))
                             .ReturnsAsync(_fakeRoles.First());

            // Act
            var result = await _accountsService.IsEditUserValidAsync(modelState, editUser);

            // Assert
            Assert.True(result);
            Assert.True(modelState.IsValid);
        }

        [Fact]
        public async Task LogoutAsync_WhenCalled_SignOut()
        {
            // Arrange
            _signInManagerMock.Setup(sm => sm.SignOutAsync())
                              .Returns(Task.CompletedTask);

            // Act
            await _accountsService.LogoutAsync();

            // Assert
            _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangeRolesToUserAsync_WhenCalled_ChangesRolesOfUsers()
        {
            // Arrange
            var oldRoleName = _fakeRoles.First().Name;
            var userRoleName = Constants.UserRoleName;

            _userManagerMock.Setup(service => service.GetUsersInRoleAsync(oldRoleName!))
                             .ReturnsAsync(_fakeUsers);

            _userManagerMock.Setup(service => service.RemoveFromRoleAsync(It.IsAny<User>(), oldRoleName!))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(service => service.AddToRoleAsync(It.IsAny<User>(), userRoleName))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            await _accountsService.ChangeRolesToUserAsync(oldRoleName!);

            // Assert
            _userManagerMock.Verify(um => um.GetUsersInRoleAsync(oldRoleName!), Times.Once);
            _userManagerMock.Verify(um => um.RemoveFromRoleAsync(It.IsAny<User>(), oldRoleName!), Times.Exactly(_fakeUsers.Count));
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), userRoleName), Times.Exactly(_fakeUsers.Count));
        }

        [Fact]
        public async Task ExportAllToExcelAsync_ReturnsMemoryStreamWithUsersData()
        {
            // Arrange
            var fakeRolesNames = _fakeRoles.Select(r => r.Name)
                                           .ToList();

            var fakeStream = new MemoryStream();

            _userManagerMock.Setup(um => um.Users)
                            .Returns(new TestAsyncEnumerable<User>(_fakeUsers).AsQueryable());

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                            .ReturnsAsync(fakeRolesNames);

            _excelServiceMock.Setup(service => service.ExportUsers(It.IsAny<List<UserViewModel>>()))
                             .Returns(fakeStream);

            // Act
            var result = await _accountsService.ExportAllToExcelAsync();

            // Assert
            Assert.IsType<MemoryStream>(result);
            _userManagerMock.Verify(repo => repo.Users, Times.Once);
            _excelServiceMock.Verify(service => service.ExportUsers(It.IsAny<List<UserViewModel>>()), Times.Once);
        }
    }
}
