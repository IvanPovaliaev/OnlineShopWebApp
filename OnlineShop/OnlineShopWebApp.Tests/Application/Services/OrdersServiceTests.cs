using AutoMapper;
using LinqSpecs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Application.Services;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Application.Services
{
    public class OrdersServiceTests
    {
        private readonly Mock<IOrdersRepository> _ordersRepositoryMock;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly IMapper _mapper;
        private readonly OrdersService _ordersService;

        private readonly FakerProvider _fakerProvider;
        private readonly List<Order> _fakeOrders;
        private readonly List<OrderViewModel> _fakeOrderViewModels;

        public OrdersServiceTests(Mock<IOrdersRepository> ordersRepositoryMock, IMapper mapper, FakerProvider fakerProvider, Mock<IExcelService> excelServiceMock)
        {
            _ordersRepositoryMock = ordersRepositoryMock;
            _excelServiceMock = excelServiceMock;
            _mapper = mapper;

            _ordersService = new OrdersService(_ordersRepositoryMock.Object, _excelServiceMock.Object, _mapper);

            _fakerProvider = fakerProvider;
            _fakeOrders = _fakerProvider.FakeOrders;
            _fakeOrderViewModels = _fakeOrders.Select(_mapper.Map<OrderViewModel>)
                                              .ToList();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsOrderViewModelsList()
        {
            // Arrange
            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()))
                                 .ReturnsAsync(_fakeOrders);

            // Act
            var result = await _ordersService.GetAllAsync();

            // Assert
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

            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()), Times.Once);
        }


        [Fact]
        public async Task GetLastAsync_WhenOrdersExist_ReturnLastOrderForUser()
        {
            // Arrange
            var userId = _fakeOrders[0].UserId;
            var expectedOrders = _fakeOrders.Where(o => o.UserId == userId)
                                            .ToList();

            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()))
                                 .ReturnsAsync(expectedOrders);

            // Act
            var result = await _ordersService.GetLastAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);

            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()), Times.Once);
        }

        [Fact]
        public async Task GetLastAsync_WhenUserHasNoOrders_ReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()))
                                 .ReturnsAsync([]);

            // Act
            var result = await _ordersService.GetLastAsync(userId);

            // Assert
            Assert.Null(result);
            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCreationSuccess_ReturnGuid()
        {
            // Arrange
            var userId = _fakeOrderViewModels.First().UserId;
            var deliveryInfo = _fakeOrderViewModels.First().Info;
            var positions = _fakerProvider.CartPositionFaker.Generate(2);
            var expectedId = Guid.NewGuid();

            _ordersRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>()))
                                 .ReturnsAsync(expectedId);

            // Act
            var result = await _ordersService.CreateAsync(userId, deliveryInfo, positions);

            // Assert
            Assert.Equal(expectedId, result);
            _ordersRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCreationFailed_ReturnsNull()
        {
            // Arrange
            var userId = _fakeOrderViewModels.First().UserId;
            var deliveryInfo = _fakeOrderViewModels.First().Info;
            var positions = _fakerProvider.CartPositionFaker.Generate(2);

            _ordersRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>()))
                                 .ReturnsAsync((Guid?)null!);

            // Act
            var result = await _ordersService.CreateAsync(userId, deliveryInfo, positions);

            // Assert
            Assert.Null(result);
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
            var positions = _fakerProvider.CartPositionFaker.Generate(2);

            // Act
            var result = _ordersService.IsCreationValid(modelState, positions);

            // Assert
            Assert.True(result);
            Assert.Equal(0, modelState.ErrorCount);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenUpdateSuccess_ReturnTrue()
        {
            // Arrange
            var orderId = _fakeOrders.First().Id;
            var newStatus = OrderStatusViewModel.Confirmed;

            _ordersRepositoryMock.Setup(repo => repo.UpdateStatusAsync(orderId, (OrderStatus)newStatus))
                                 .ReturnsAsync(true);

            // Act
            var result = await _ordersService.UpdateStatusAsync(orderId, newStatus);

            // Assert
            Assert.True(result);
            _ordersRepositoryMock.Verify(repo => repo.UpdateStatusAsync(orderId, (OrderStatus)newStatus), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenUpdateFalse_ReturnFalse()
        {
            // Arrange
            var orderId = _fakeOrders.First().Id;
            var newStatus = OrderStatusViewModel.Confirmed;

            _ordersRepositoryMock.Setup(repo => repo.UpdateStatusAsync(orderId, (OrderStatus)newStatus))
                                 .ReturnsAsync(false);

            // Act
            var result = await _ordersService.UpdateStatusAsync(orderId, newStatus);

            // Assert
            Assert.False(result);
            _ordersRepositoryMock.Verify(repo => repo.UpdateStatusAsync(orderId, (OrderStatus)newStatus), Times.Once);
        }

        [Fact]
        public async Task ExportAllToExcelAsync_WhenCalled_ShouldReturnMemoryStream()
        {
            // Arrange
            var memoryStream = new MemoryStream();

            _ordersRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()))
                                 .ReturnsAsync(_fakeOrders);

            _excelServiceMock.Setup(service => service.ExportOrders(It.IsAny<List<OrderViewModel>>()))
                             .Returns(memoryStream);

            // Act
            var result = await _ordersService.ExportAllToExcelAsync();

            // Assert
            Assert.IsType<MemoryStream>(result);
            _ordersRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<Specification<Order>>()), Times.Once);
            _excelServiceMock.Verify(service => service.ExportOrders(It.IsAny<List<OrderViewModel>>()), Times.Once);
        }
    }

}
