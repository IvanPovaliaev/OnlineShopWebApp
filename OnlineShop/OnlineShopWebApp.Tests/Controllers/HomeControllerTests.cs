﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Domain.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IProductsService> _productsServiceMock;
        private readonly HomeController _controller;
        private readonly IMapper _mapper;
        private readonly List<Product> _fakeProducts;

        public HomeControllerTests(IMapper mapper, Mock<IProductsService> productsServiceMock, FakerProvider fakerProvider)
        {
            _productsServiceMock = productsServiceMock;
            _controller = new HomeController(_productsServiceMock.Object);

            _mapper = mapper;
            _fakeProducts = fakerProvider.FakeProducts;
        }

        [Fact]
        public async Task Index_ReturnsViewWithProducts()
        {
            // Arrange
            var expectedCount = _fakeProducts.Count;
            var fakeProducts = _fakeProducts.Select(_mapper.Map<ProductViewModel>)
                                            .ToList();

            _productsServiceMock.Setup(s => s.GetAllAsync())
                                .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(viewResult.Model);
            Assert.Equal(expectedCount, model.Count());
        }

        [Fact]
        public void About_ReturnsViewResult()
        {
            // Act
            var result = _controller.About();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Contacts_ReturnsViewResult()
        {
            // Act
            var result = _controller.Contacts();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Delivery_ReturnsViewResult()
        {
            // Act
            var result = _controller.Delivery();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData(404)]
        [InlineData(null)]
        public void Error_WhenCalled_ReturnsCorrectViewBasedOnStatusCode(int? statusCode)
        {
            // Act
            var result = _controller.Error(statusCode);

            // Assert
            if (statusCode == 404)
            {
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("NotFound", viewResult.ViewName);
            }
            else
            {
                Assert.IsType<ViewResult>(result);
            }
        }
    }
}
