using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
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
        private readonly string? _userId;
        private readonly Mock<IComparisonsService> _comparisonsServiceMock;
        private readonly ComparisonViewComponent _viewComponent;
        private readonly List<ComparisonProductViewModel> _fakeComparisonProducts;

        public ComparisonViewComponentTests(Mock<IComparisonsService> comparisonsServiceMock, IMapper mapper, FakerProvider fakerProvider, Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            _userId = fakerProvider.UserId;
            _comparisonsServiceMock = comparisonsServiceMock;
            _viewComponent = new ComparisonViewComponent(_comparisonsServiceMock.Object, httpContextAccessorMock.Object);

            _fakeComparisonProducts = fakerProvider.FakeComparisonProducts.Select(mapper.Map<ComparisonProductViewModel>)
                                                                          .ToList();
        }

        [Fact]
        public async Task InvokeAsync_WhenComparisonsExist_ReturnsCorrectCountInView()
        {
            // Arrange
            var expectedQuantity = _fakeComparisonProducts.Count;
            _comparisonsServiceMock.Setup(s => s.GetAllAsync(_userId!))
                                   .ReturnsAsync(_fakeComparisonProducts);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Comparison", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(expectedQuantity, factQuantity);
            _comparisonsServiceMock.Verify(s => s.GetAllAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoComparisons_ReturnsZeroCountInView()
        {
            // Arrange
            var emptyComparisons = new List<ComparisonProductViewModel>();
            _comparisonsServiceMock.Setup(s => s.GetAllAsync(_userId!))
                                   .ReturnsAsync(emptyComparisons);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Comparison", viewResult.ViewName);
            var factQuantity = Convert.ToInt32(viewResult.ViewData!.Model);
            Assert.Equal(0, factQuantity);
            _comparisonsServiceMock.Verify(s => s.GetAllAsync(_userId!), Times.Once);
        }
    }
}
