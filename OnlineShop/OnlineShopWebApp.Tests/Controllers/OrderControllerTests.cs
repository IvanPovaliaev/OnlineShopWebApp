﻿using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<CartsService> _cartsServiceMock;
        private readonly Mock<OrdersService> _ordersServiceMock;
        private readonly OrderController _controller;
        private readonly IMapper _mapper;
        private readonly Cart _fakeCart;
        private readonly Faker<UserDeliveryInfo> _userDeliveryInfoFaker;

        public OrderControllerTests()
        {
            _cartsServiceMock = new Mock<CartsService>(null!, null!, null!);
            _ordersServiceMock = new Mock<OrdersService>(null!, null!, null!);
            _controller = new OrderController(_cartsServiceMock.Object, _ordersServiceMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _userDeliveryInfoFaker = FakerProvider.UserDeliveryInfoFaker;
            _fakeCart = FakerProvider.FakeCart;
        }

        [Fact]
        public async Task Create_WhenModelIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var deliveryInfo = _mapper.Map<UserDeliveryInfoViewModel>(_userDeliveryInfoFaker.Generate());
            var cart = _fakeCart;
            var modelState = new ModelStateDictionary();

            _cartsServiceMock.Setup(s => s.GetAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(cart);

            _ordersServiceMock.Setup(s => s.IsCreationValid(modelState, _fakeCart.Positions))
                              .Returns(false);

            // Act
            var result = await _controller.Create(deliveryInfo);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _cartsServiceMock.Verify(s => s.GetAsync(It.IsAny<Guid>()), Times.Once);
            _ordersServiceMock.Verify(s => s.IsCreationValid(modelState, _fakeCart.Positions), Times.Once);
            _ordersServiceMock.Verify(s => s.CreateAsync(It.IsAny<Guid>(), deliveryInfo, _fakeCart.Positions), Times.Never);
            _ordersServiceMock.Verify(s => s.GetLastAsync(It.IsAny<Guid>()), Times.Never);
            _cartsServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Create_WhenModelIsValid_CreatesOrder_ReturnsOrderView()
        {
            // Arrange
            var deliveryInfo = _mapper.Map<UserDeliveryInfoViewModel>(_userDeliveryInfoFaker.Generate());
            var cart = _fakeCart;
            var modelState = new ModelStateDictionary();

            _cartsServiceMock.Setup(s => s.GetAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(cart);

            _ordersServiceMock.Setup(s => s.IsCreationValid(modelState, _fakeCart.Positions))
                              .Returns(true);

            _ordersServiceMock.Setup(s => s.CreateAsync(It.IsAny<Guid>(), deliveryInfo, _fakeCart.Positions))
                              .Returns(Task.CompletedTask);

            var fakeOrder = new OrderViewModel();
            _ordersServiceMock.Setup(s => s.GetLastAsync(It.IsAny<Guid>()))
                              .ReturnsAsync(fakeOrder);

            _cartsServiceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>()))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(deliveryInfo);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OrderViewModel>(viewResult.Model);
            Assert.Equal(fakeOrder, model);
            _cartsServiceMock.Verify(s => s.GetAsync(It.IsAny<Guid>()), Times.Once);
            _ordersServiceMock.Verify(s => s.IsCreationValid(modelState, _fakeCart.Positions), Times.Once);
            _ordersServiceMock.Verify(s => s.CreateAsync(It.IsAny<Guid>(), deliveryInfo, _fakeCart.Positions), Times.Once);
            _ordersServiceMock.Verify(s => s.GetLastAsync(It.IsAny<Guid>()), Times.Once);
            _cartsServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
