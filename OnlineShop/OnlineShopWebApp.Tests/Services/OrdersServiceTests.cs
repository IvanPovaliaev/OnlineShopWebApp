﻿using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
{
    public class OrdersServiceTests
    {
        private readonly Mock<IOrdersRepository> _ordersRepositoryMock;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrdersService _ordersService;

        private readonly List<Order> _fakeOrders;
        private readonly List<OrderViewModel> _fakeOrderViewModels;
        private readonly Faker<UserDeliveryInfo> _userDeliveryInfoFaker;
        private const int OrdersCount = 10;

        public OrdersServiceTests()
        {
            _ordersRepositoryMock = new Mock<IOrdersRepository>();
            _excelServiceMock = new Mock<IExcelService>();
            _mapperMock = InitializeMapperMock();
            _ordersService = new OrdersService(_ordersRepositoryMock.Object, _excelServiceMock.Object, _mapperMock.Object);

            _userDeliveryInfoFaker = new Faker<UserDeliveryInfo>()
                .RuleFor(i => i.Id, f => f.Random.Guid())
                .RuleFor(i => i.City, f => f.Address.City())
                .RuleFor(i => i.Address, f => f.Address.FullAddress())
                .RuleFor(i => i.PostCode, f => f.Address.ZipCode())
                .RuleFor(i => i.FullName, f => f.Name.FullName())
                .RuleFor(i => i.Email, f => f.Internet.Email())
                .RuleFor(i => i.Phone, f => f.Phone.PhoneNumber());

            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.Id, f => f.Random.Guid())
                .RuleFor(o => o.UserId, f => f.Random.Guid())
                .RuleFor(o => o.CreationDate, f => f.Date.Past())
                .RuleFor(o => o.Info, f => _userDeliveryInfoFaker.Generate());

            _fakeOrders = orderFaker.Generate(OrdersCount);
            _fakeOrderViewModels = _fakeOrders.Select(order => new OrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                CreationDate = order.CreationDate,
                Status = (OrderStatusViewModel)order.Status,
                Info = new UserDeliveryInfoViewModel()
                {
                    City = order.Info.City,
                    Address = order.Info.Address,
                    PostCode = order.Info.PostCode,
                    FullName = order.Info.FullName,
                    Email = order.Info.Email,
                    Phone = order.Info.Phone,
                }
            }).ToList();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsOrderViewModelsList()
        {
            // Arrange
            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                 .ReturnsAsync(_fakeOrders);

            var result = await _ordersService.GetAllAsync();

            Assert.Equal(_fakeOrderViewModels.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(_fakeOrderViewModels[i].Id, result[i].Id);
                Assert.Equal(_fakeOrderViewModels[i].CreationDate, result[i].CreationDate);
                Assert.Equal(_fakeOrderViewModels[i].UserId, result[i].UserId);
                Assert.Equal(_fakeOrderViewModels[i].Status, result[i].Status);
                Assert.Equal(_fakeOrderViewModels[i].Info.City, result[i].Info.City);
                Assert.Equal(_fakeOrderViewModels[i].Info.Address, result[i].Info.Address);
                Assert.Equal(_fakeOrderViewModels[i].Info.PostCode, result[i].Info.PostCode);
                Assert.Equal(_fakeOrderViewModels[i].Info.FullName, result[i].Info.FullName);
                Assert.Equal(_fakeOrderViewModels[i].Info.Email, result[i].Info.Email);
                Assert.Equal(_fakeOrderViewModels[i].Info.Phone, result[i].Info.Phone);
            }

            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }


        [Fact]
        public async Task GetLastAsync_WhenOrdersExist_ReturnLastOrderForUser()
        {
            // Arrange
            var userId = _fakeOrders[0].UserId;
            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                 .ReturnsAsync(_fakeOrders);

            // Act
            var result = await _ordersService.GetLastAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);

            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetLastAsync_WhenUserHasNoOrders_ReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                 .ReturnsAsync(_fakeOrders);

            // Act
            var result = await _ordersService.GetLastAsync(userId);

            // Assert
            Assert.Null(result);
            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_InvokeRepositoryCreateAsyncWithCorrectOrder()
        {
            // Arrange
            var userId = _fakeOrderViewModels.First().UserId;
            var deliveryInfo = _fakeOrderViewModels.First().Info;
            var positions = new List<CartPosition>
            {
                new()
                {
                    Product = new Product { Id = Guid.NewGuid(), Name = "Product1" },
                    Quantity = 1
                }
            };

            _mapperMock.Setup(mapper => mapper.Map<UserDeliveryInfo>(deliveryInfo))
                       .Returns(_fakeOrders.First().Info);

            _ordersRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>()))
                                 .Returns(Task.CompletedTask);

            // Act
            await _ordersService.CreateAsync(userId, deliveryInfo, positions);

            // Assert
            _ordersRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public void IsCreationValid_WhenPositionsAreEmpty_ReturnsFalseWithModelErrors()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            var positions = new List<CartPosition>();

            // Act
            var result = _ordersService.IsCreationValid(modelState, positions);

            // Assert
            Assert.False(result);
            Assert.Contains(modelState, m => m.Value!.Errors.Any());
        }

        [Fact]
        public void IsCreationValid_WhenPositionsAreNotEmpty_ReturnsTrueWithoutModelErrors()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            var positions = new List<CartPosition>
            {
                new CartPosition { Product = new Product { Id = Guid.NewGuid(), Name = "Product1" }, Quantity = 1 }
            };

            // Act
            var result = _ordersService.IsCreationValid(modelState, positions);

            // Assert
            Assert.True(result);
            Assert.Equal(0, modelState.ErrorCount);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenCalled_CallUpdateStatusInRepository()
        {
            // Arrange
            var orderId = _fakeOrders.First().Id;
            var newStatus = OrderStatusViewModel.Confirmed;

            _ordersRepositoryMock.Setup(repo => repo.UpdateStatusAsync(orderId, (OrderStatus)newStatus))
                                 .Returns(Task.CompletedTask);

            // Act
            await _ordersService.UpdateStatusAsync(orderId, newStatus);

            // Assert
            _ordersRepositoryMock.Verify(repo => repo.UpdateStatusAsync(orderId, (OrderStatus)newStatus), Times.Once);
        }

        [Fact]
        public async Task ExportAllToExcelAsync_WhenCalled_ShouldReturnMemoryStream()
        {
            // Arrange
            var memoryStream = new MemoryStream();

            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync())
                                 .ReturnsAsync(_fakeOrders);

            _excelServiceMock.Setup(service => service.ExportOrders(It.IsAny<List<OrderViewModel>>()))
                             .Returns(memoryStream);

            // Act
            var result = await _ordersService.ExportAllToExcelAsync();

            // Assert
            Assert.IsType<MemoryStream>(result);
            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _excelServiceMock.Verify(service => service.ExportOrders(It.IsAny<List<OrderViewModel>>()), Times.Once);
        }

        private Mock<IMapper> InitializeMapperMock()
        {
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<OrderViewModel>(It.IsAny<Order>()))
                      .Returns((Order order) => new OrderViewModel
                      {
                          Id = order.Id,
                          UserId = order.UserId,
                          CreationDate = order.CreationDate,
                          Status = (OrderStatusViewModel)order.Status,
                          Info = new UserDeliveryInfoViewModel()
                          {
                              City = order.Info.City,
                              Address = order.Info.Address,
                              PostCode = order.Info.PostCode,
                              FullName = order.Info.FullName,
                              Email = order.Info.Email,
                              Phone = order.Info.Phone,
                          }
                      });
            return mapperMock;
        }
    }

}
