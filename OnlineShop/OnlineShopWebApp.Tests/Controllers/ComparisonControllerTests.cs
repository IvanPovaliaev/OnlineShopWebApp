//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using OnlineShop.Db.Models;
//using OnlineShopWebApp.Controllers;
//using OnlineShopWebApp.Models;
//using OnlineShopWebApp.Services;
//using OnlineShopWebApp.Tests.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace OnlineShopWebApp.Tests.Controllers
//{
//    public class ComparisonControllerTests
//    {
//        private readonly Mock<ComparisonsService> _comparisonsServiceMock;
//        private readonly ComparisonController _controller;
//        private readonly IMapper _mapper;
//        private readonly List<ComparisonProduct> _fakeComparisonProducts;

//        public ComparisonControllerTests()
//        {
//            _comparisonsServiceMock = new Mock<ComparisonsService>(null!, null!, null!);
//            _controller = new ComparisonController(_comparisonsServiceMock.Object);

//            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
//            _mapper = config.CreateMapper();

//            _fakeComparisonProducts = FakerProvider.FakeComparisonProducts;
//        }

//        [Fact]
//        public async Task Index_WhenCalled_ReturnsViewWithComparisonGroupsAndCategory()
//        {
//            // Arrange
//            var fakeComparisonGroups = _fakeComparisonProducts.Select(_mapper.Map<ComparisonProductViewModel>)
//                                        .ToLookup(c => c.Product.Category);

//            _comparisonsServiceMock.Setup(s => s.GetGroupsAsync(It.IsAny<string>()))
//                                   .ReturnsAsync(fakeComparisonGroups);

//            // Act
//            var result = await _controller.Index(null);

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var model = (ValueTuple<ILookup<ProductCategoriesViewModel, ComparisonProductViewModel>, ProductCategoriesViewModel?>)viewResult.Model!;
//            Assert.NotNull(viewResult.Model);
//            Assert.Equal(fakeComparisonGroups, model.Item1);
//        }

//        [Fact]
//        public async Task Add_AddsProductToComparisons_ReturnsPartialView()
//        {
//            // Arrange
//            var productId = Guid.NewGuid();

//            // Act
//            var result = await _controller.Add(productId);

//            // Assert
//            _comparisonsServiceMock.Verify(s => s.CreateAsync(productId, It.IsAny<string>()), Times.Once);
//            var partialViewResult = Assert.IsType<PartialViewResult>(result);
//            Assert.Equal("_NavUserIcons", partialViewResult.ViewName);
//        }

//        [Fact]
//        public async Task Delete_RemovesProductFromComparisons_ReturnsRedirectToIndex()
//        {
//            // Arrange
//            var comparisonId = Guid.NewGuid();

//            // Act
//            var result = await _controller.Delete(comparisonId);

//            // Assert
//            _comparisonsServiceMock.Verify(s => s.DeleteAsync(comparisonId), Times.Once);
//            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//            Assert.Equal("Index", redirectResult.ActionName);
//        }

//        [Fact]
//        public async Task DeleteAll_RemovesAllComparisonsForUser_ReturnsRedirectToIndex()
//        {
//            // Act
//            var result = await _controller.DeleteAll();

//            // Assert
//            _comparisonsServiceMock.Verify(s => s.DeleteAllAsync(It.IsAny<string>()), Times.Once);
//            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//            Assert.Equal("Index", redirectResult.ActionName);
//        }
//    }
//}
