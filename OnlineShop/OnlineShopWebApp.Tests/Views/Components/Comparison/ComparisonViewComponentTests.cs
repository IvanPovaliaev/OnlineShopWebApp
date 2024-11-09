using AutoMapper;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using OnlineShopWebApp.Views.Shared.Components.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Views.Components.Comparison
{
    public class ComparisonViewComponentTests
    {
        private readonly Mock<ComparisonsService> _comparisonsServiceMock;
        private readonly ComparisonViewComponent _viewComponent;
        private readonly List<ComparisonProductViewModel> _fakeComparisonProducts;

        public ComparisonViewComponentTests()
        {
            _comparisonsServiceMock = new Mock<ComparisonsService>(null!, null!, null!);
            _viewComponent = new ComparisonViewComponent(_comparisonsServiceMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            var mapper = config.CreateMapper();

            _fakeComparisonProducts = FakerProvider.FakeComparisonProducts.Select(mapper.Map<ComparisonProductViewModel>)
                                                                          .ToList();
        }

        [Fact]
        public async Task InvokeAsync_WhenComparisonsExist_ReturnsCorrectCountInView()
        {
            // Arrange
            var expectedQuantity = _fakeComparisonProducts.Count;
            _comparisonsServiceMock.Setup(s => s.GetAllAsync(It.IsAny<string>()))
                                   .ReturnsAsync(_fakeComparisonProducts);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Comparison", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(expectedQuantity, factQuantity);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoComparisons_ReturnsZeroCountInView()
        {
            // Arrange
            var emptyComparisons = new List<ComparisonProductViewModel>();
            _comparisonsServiceMock.Setup(s => s.GetAllAsync(It.IsAny<string>()))
                                   .ReturnsAsync(emptyComparisons);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Comparison", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
        }
    }
}
