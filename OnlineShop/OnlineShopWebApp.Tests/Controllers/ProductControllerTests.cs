﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<ProductsService> _productsServiceMock;
        private readonly ProductController _controller;
        private readonly IMapper _mapper;

        private readonly List<Product> _fakeProducts;

        public ProductControllerTests()
        {
            _productsServiceMock = new Mock<ProductsService>(null!, null!, null!, null!);
            _controller = new ProductController(_productsServiceMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _fakeProducts = FakerProvider.FakeProducts;
        }

        [Fact]
        public async Task Index_WhenProductExists_ReturnsViewWithProduct()
        {
            // Arrange
            var fakeProduct = _mapper.Map<ProductViewModel>(_fakeProducts.First());

            _productsServiceMock.Setup(s => s.GetViewModelAsync(fakeProduct.Id))
                                .ReturnsAsync(fakeProduct);

            // Act
            var result = await _controller.Index(fakeProduct.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductViewModel>(viewResult.Model);
            Assert.Equal(fakeProduct, model);
        }

        [Fact]
        public async Task Index_WhenProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productsServiceMock.Setup(s => s.GetViewModelAsync(productId))
                                .ReturnsAsync((ProductViewModel?)null!);

            // Act
            var result = await _controller.Index(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Category_WhenCategoryExists_ReturnsViewWithProducts()
        {
            // Arrange            
            var fakeProducts = _fakeProducts.Select(_mapper.Map<ProductViewModel>)
                                            .ToList();

            var category = fakeProducts.First().Category;

            _productsServiceMock.Setup(s => s.GetAllAsync(It.IsAny<ProductCategoriesViewModel>()))
                                .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.Category(category);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<(List<ProductViewModel>, ProductCategoriesViewModel)>(viewResult.Model);
            Assert.Equal(fakeProducts, model.Item1);
            Assert.Equal(category, model.Item2);
        }

        [Fact]
        public async Task All_WhenCalled_ReturnsViewWithAllProducts()
        {
            // Arrange
            var fakeProducts = _fakeProducts.Select(_mapper.Map<ProductViewModel>)
                                            .ToList();

            _productsServiceMock.Setup(s => s.GetAllAsync())
                                .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.All();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<ProductViewModel>>(viewResult.Model);
            Assert.Equal(fakeProducts, model);
        }

        [Fact]
        public async Task SearchResult_WhenSearchQueryHasResults_ReturnsViewWithProducts()
        {
            // Arrange
            var searchQuery = "Test";
            var fakeProducts = _fakeProducts.Select(_mapper.Map<ProductViewModel>)
                                            .ToList();

            _productsServiceMock.Setup(s => s.GetAllFromSearchAsync(searchQuery))
                                .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.SearchResult(searchQuery);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<(List<ProductViewModel>, string)>(viewResult.Model);
            Assert.Equal(fakeProducts, model.Item1);
            Assert.Equal(searchQuery, model.Item2);
        }

        [Fact]
        public async Task SearchResult_WhenSearchQueryHasNoResults_ReturnsEmptyList()
        {
            // Arrange
            var searchQuery = "Nonexistent";
            var fakeProducts = new List<ProductViewModel>();

            _productsServiceMock.Setup(s => s.GetAllFromSearchAsync(searchQuery))
                                .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.SearchResult(searchQuery);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<(List<ProductViewModel>, string)>(viewResult.Model);
            Assert.Empty(model.Item1);
            Assert.Equal(searchQuery, model.Item2);
        }
    }
}