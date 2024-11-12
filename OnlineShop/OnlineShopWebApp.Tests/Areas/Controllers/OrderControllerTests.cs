using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Controllers;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Areas.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<OrdersService> _ordersServiceMock;
        private readonly OrderController _controller;
        private readonly IMapper _mapper;
        private readonly List<Order> _fakeOrders;

        public OrderControllerTests(IMapper mapper, FakerProvider fakerProvider)
        {
            _ordersServiceMock = new Mock<OrdersService>(null!, null!, null!);
            _controller = new OrderController(_ordersServiceMock.Object);

            _mapper = mapper;
            _fakeOrders = fakerProvider.FakeOrders;
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithOrders()
        {
            // Arrange
            var fakeOrders = _fakeOrders.Select(_mapper.Map<OrderViewModel>)
                                        .ToList();
            _ordersServiceMock.Setup(s => s.GetAllAsync())
                              .ReturnsAsync(fakeOrders);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<OrderViewModel>>(viewResult.Model);
            Assert.Equal(fakeOrders, model);
        }

        [Fact]
        public async Task UpdateStatus_WhenCalled_UpdatesOrderStatusAndRedirectsToIndex()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var newStatus = OrderStatusViewModel.Delivering;

            // Act
            var result = await _controller.UpdateStatus(orderId, newStatus);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _ordersServiceMock.Verify(s => s.UpdateStatusAsync(orderId, newStatus), Times.Once);
        }

        [Fact]
        public async Task ExportToExcel_WhenCalled_ReturnsExcelFileStream()
        {
            // Arrange
            var fakeStream = new MemoryStream();
            _ordersServiceMock.Setup(s => s.ExportAllToExcelAsync())
                              .ReturnsAsync(fakeStream);

            // Act
            var result = await _controller.ExportToExcel();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(Formats.ExcelFileContentType, fileResult.ContentType);
        }
    }
}
