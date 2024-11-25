﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Tests.Helpers;
using OnlineShopWebApp.Views.Shared.Components.Cart;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Views.Components.Avatar
{
    public class AvatarViewComponentTests
    {
        private readonly string? _userId;
        private readonly Mock<IAccountsService> _accountsServiceMock;
        private readonly AvatarViewComponent _viewComponent;
        private readonly List<UserViewModel> _fakeUsersViewModel;

        public AvatarViewComponentTests(Mock<IAccountsService> accountsServiceMock, IMapper mapper, FakerProvider fakerProvider, Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            _userId = fakerProvider.UserId;
            _accountsServiceMock = accountsServiceMock;
            _viewComponent = new AvatarViewComponent(_accountsServiceMock.Object, httpContextAccessorMock.Object);

            _fakeUsersViewModel = fakerProvider.FakeUsers.Select(mapper.Map<UserViewModel>)
                                                                      .ToList();
        }

        [Fact]
        public async Task InvokeAsync_WhenUserExist_ReturnsUserAvatar()
        {
            // Arrange
            var expectedUserVM = _fakeUsersViewModel.First();
            _accountsServiceMock.Setup(s => s.GetAsync(_userId!))
                                 .ReturnsAsync(expectedUserVM);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Avatar", viewResult.ViewName);

            var factUrl = viewResult.ViewData!.Model;
            Assert.Equal(expectedUserVM.AvatarUrl, factUrl);
            _accountsServiceMock.Verify(s => s.GetAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenUserNotExist_ReturnsNull()
        {
            // Arrange
            _accountsServiceMock.Setup(s => s.GetAsync(_userId!))
                                 .ReturnsAsync((UserViewModel)null!);

            // Act
            var result = await _viewComponent.InvokeAsync();


            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Avatar", viewResult.ViewName);

            var factUrl = viewResult.ViewData!.Model;
            Assert.Null(factUrl);
            _accountsServiceMock.Verify(s => s.GetAsync(_userId!), Times.Once);
        }
    }
}