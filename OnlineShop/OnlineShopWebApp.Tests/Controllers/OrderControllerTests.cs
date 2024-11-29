using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Domain.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Tests.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly string _userId;
        private readonly Mock<ICartsService> _cartsServiceMock;
        private readonly Mock<IOrdersService> _ordersServiceMock;
        private readonly OrderController _controller;
        private readonly IMapper _mapper;
        private readonly Cart _fakeCart;
        private readonly Faker<UserDeliveryInfo> _userDeliveryInfoFaker;

        public OrderControllerTests(IMapper mapper, Mock<ICartsService> cartsServiceMock, Mock<IOrdersService> ordersServiceMock, Mock<IHttpContextAccessor> httpContextAccessorMock, FakerProvider fakerProvider)
        {
            _userId = fakerProvider.UserId;
            _cartsServiceMock = cartsServiceMock;
            _ordersServiceMock = ordersServiceMock;
            _controller = new OrderController(_cartsServiceMock.Object, _ordersServiceMock.Object, httpContextAccessorMock.Object);

            _mapper = mapper;
            _userDeliveryInfoFaker = fakerProvider.UserDeliveryInfoFaker;
            _fakeCart = fakerProvider.FakeCart;
        }

        [Fact]
        public async Task Create_WhenModelIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var deliveryInfo = _mapper.Map<UserDeliveryInfoViewModel>(_userDeliveryInfoFaker.Generate());
            var cart = _fakeCart;
            var modelState = new ModelStateDictionary();

            _cartsServiceMock.Setup(s => s.GetAsync(_userId!))
                             .ReturnsAsync(cart);

            _ordersServiceMock.Setup(s => s.IsCreationValid(modelState, _fakeCart.Positions))
                              .Returns(false);

            // Act
            var result = await _controller.Create(deliveryInfo);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _cartsServiceMock.Verify(s => s.GetAsync(_userId!), Times.Once);
            _ordersServiceMock.Verify(s => s.IsCreationValid(modelState, _fakeCart.Positions), Times.Once);
            _ordersServiceMock.Verify(s => s.CreateAsync(It.IsAny<string>(), deliveryInfo, _fakeCart.Positions), Times.Never);
            _ordersServiceMock.Verify(s => s.GetLastAsync(It.IsAny<string>()), Times.Never);
            _cartsServiceMock.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Create_WhenModelIsValid_CreatesOrder_ReturnsOrderView()
        {
            // Arrange
            var deliveryInfo = _mapper.Map<UserDeliveryInfoViewModel>(_userDeliveryInfoFaker.Generate());
            var cart = _fakeCart;
            var modelState = new ModelStateDictionary();

            _cartsServiceMock.Setup(s => s.GetAsync(_userId!))
                             .ReturnsAsync(cart);

            _ordersServiceMock.Setup(s => s.IsCreationValid(modelState, _fakeCart.Positions))
                              .Returns(true);

            _ordersServiceMock.Setup(s => s.CreateAsync(_userId!, deliveryInfo, _fakeCart.Positions))
                              .Returns(Task.CompletedTask);

            var fakeOrder = new OrderViewModel();
            _ordersServiceMock.Setup(s => s.GetLastAsync(_userId!))
                              .ReturnsAsync(fakeOrder);

            _cartsServiceMock.Setup(s => s.DeleteAsync(_userId!))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(deliveryInfo);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OrderViewModel>(viewResult.Model);
            Assert.Equal(fakeOrder, model);
            _cartsServiceMock.Verify(s => s.GetAsync(_userId!), Times.Once);
            _ordersServiceMock.Verify(s => s.IsCreationValid(modelState, _fakeCart.Positions), Times.Once);
            _ordersServiceMock.Verify(s => s.CreateAsync(_userId!, deliveryInfo, _fakeCart.Positions), Times.Once);
            _ordersServiceMock.Verify(s => s.GetLastAsync(_userId!), Times.Once);
            _cartsServiceMock.Verify(s => s.DeleteAsync(_userId!), Times.Once);
        }
    }
}
