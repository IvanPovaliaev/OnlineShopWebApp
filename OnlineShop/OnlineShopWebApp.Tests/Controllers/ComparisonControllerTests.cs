using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Controllers
{
    public class ComparisonControllerTests
    {
        private readonly string? _userId;
        private readonly Mock<IComparisonsService> _comparisonsServiceMock;
        private readonly ComparisonController _controller;
        private readonly IMapper _mapper;
        private readonly List<ComparisonProduct> _fakeComparisonProducts;

        public ComparisonControllerTests(IMapper mapper, Mock<IComparisonsService> comparisonsServiceMock, Mock<IHttpContextAccessor> httpContextAccessorMock, FakerProvider fakerProvider)
        {
            _userId = fakerProvider.UserId;

            _comparisonsServiceMock = comparisonsServiceMock;
            _controller = new ComparisonController(_comparisonsServiceMock.Object, httpContextAccessorMock.Object);

            _mapper = mapper;
            _fakeComparisonProducts = fakerProvider.FakeComparisonProducts;
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithComparisonGroupsAndCategory()
        {
            // Arrange
            var fakeComparisonGroups = _fakeComparisonProducts.Select(_mapper.Map<ComparisonProductViewModel>)
                                        .ToLookup(c => c.Product.Category);

            _comparisonsServiceMock.Setup(s => s.GetGroupsAsync(_userId!))
                                   .ReturnsAsync(fakeComparisonGroups);

            // Act
            var result = await _controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = (ValueTuple<ILookup<ProductCategoriesViewModel, ComparisonProductViewModel>, ProductCategoriesViewModel?>)viewResult.Model!;
            Assert.NotNull(viewResult.Model);
            Assert.Equal(fakeComparisonGroups, model.Item1);
            _comparisonsServiceMock.Verify(s => s.GetGroupsAsync(_userId!), Times.Once);
        }

        [Fact]
        public async Task Add_AddsProductToComparisons_ReturnsPartialView()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.Add(productId);

            // Assert            
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_NavUserIcons", partialViewResult.ViewName);
            _comparisonsServiceMock.Verify(s => s.CreateAsync(productId, _userId!), Times.Once);
        }

        [Fact]
        public async Task Delete_RemovesProductFromComparisons_ReturnsRedirectToIndex()
        {
            // Arrange
            var comparisonId = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(comparisonId);

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _comparisonsServiceMock.Verify(s => s.DeleteAsync(comparisonId), Times.Once);
        }

        [Fact]
        public async Task DeleteAll_RemovesAllComparisonsForUser_ReturnsRedirectToIndex()
        {
            // Act
            var result = await _controller.DeleteAll();

            // Assert            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _comparisonsServiceMock.Verify(s => s.DeleteAllAsync(_userId!), Times.Once);
        }
    }
}
