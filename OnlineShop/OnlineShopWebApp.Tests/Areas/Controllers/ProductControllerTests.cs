using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Controllers;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Areas.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<ProductsService> _productsServiceMock;
        private readonly ProductController _controller;
        private readonly IMapper _mapper;

        private readonly Faker<Product> _productFaker;
        private readonly List<Product> _fakeProducts;

        public ProductControllerTests(IMapper mapper, FakerProvider fakerProvider)
        {
            _productsServiceMock = new Mock<ProductsService>(null!, null!, null!, null!);
            _controller = new ProductController(_productsServiceMock.Object);

            _mapper = mapper;
            _productFaker = fakerProvider.ProductFaker;
            _fakeProducts = fakerProvider.FakeProducts;
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithProducts()
        {
            // Arrange
            var fakeProducts = _fakeProducts.Select(_mapper.Map<ProductViewModel>)
                                            .ToList();
            _productsServiceMock.Setup(s => s.GetAllAsync())
                                .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ProductViewModel>>(viewResult.Model);
            Assert.Equal(fakeProducts, model);
        }

        [Fact]
        public void Add_GetRequest_ReturnsView()
        {
            // Act
            var result = _controller.Add();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add_PostRequest_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var invalidProduct = new AddProductViewModel();
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Add(invalidProduct);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Add", viewResult.ViewName);
            Assert.Equal(invalidProduct, viewResult.Model);
        }

        [Fact]
        public async Task Add_PostRequest_WithValidModel_AddsProductAndRedirectsToIndex()
        {
            // Arrange
            var newProduct = _mapper.Map<AddProductViewModel>(_productFaker.Generate());

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            _productsServiceMock.Verify(s => s.AddAsync(newProduct), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_WhenCalled_ReturnsViewWithProduct()
        {
            // Arrange
            var fakeProduct = _fakeProducts.First();
            var fakeEditProduct = _mapper.Map<EditProductViewModel>(fakeProduct);
            _productsServiceMock.Setup(s => s.GetEditProductAsync(fakeProduct.Id))
                                .ReturnsAsync(fakeEditProduct);

            // Act
            var result = await _controller.Edit(fakeProduct.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditProductViewModel>(viewResult.Model);
            Assert.Equal(fakeEditProduct, model);
        }

        [Fact]
        public async Task Update_WithInvalidModel_ReturnsEditViewWithModel()
        {
            // Arrange
            var invalidProduct = new EditProductViewModel();
            var modelState = _controller.ModelState;
            modelState.AddModelError("Name", "Required");
            _productsServiceMock.Setup(s => s.IsUpdateValidAsync(modelState, invalidProduct))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(invalidProduct);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Edit", viewResult.ViewName);
            Assert.Equal(invalidProduct, viewResult.Model);
        }

        [Fact]
        public async Task Update_WithValidModel_UpdatesProductAndRedirectsToIndex()
        {
            // Arrange
            var fakeProduct = _fakeProducts.First();
            var fakeEditProduct = _mapper.Map<EditProductViewModel>(fakeProduct);
            var modelState = new ModelStateDictionary();

            _productsServiceMock.Setup(s => s.IsUpdateValidAsync(modelState, fakeEditProduct))
                                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(fakeEditProduct);

            // Assert
            _productsServiceMock.Verify(s => s.UpdateAsync(fakeEditProduct), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_WhenCalled_DeletesProductAndRedirectsToIndex()
        {
            // Arrange
            var productId = _fakeProducts.First().Id;

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            _productsServiceMock.Verify(s => s.DeleteAsync(productId), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void GetSpecificationsForm_WhenCalled_ReturnsSpecificationsFormViewComponent()
        {
            // Arrange
            var category = (ProductCategoriesViewModel)_fakeProducts.First().Category;

            // Act
            var result = _controller.GetSpecificationsForm(category);

            // Assert
            var viewComponentResult = Assert.IsType<ViewComponentResult>(result);
            var model = Assert.IsType<(Dictionary<string, string>, ProductCategoriesViewModel)>(viewComponentResult.Arguments);
            Assert.Equal(category, model.Item2);
        }

        [Fact]
        public async Task ExportToExcel_WhenCalled_ReturnsExcelFileStream()
        {
            // Arrange
            var fakeStream = new MemoryStream();
            _productsServiceMock.Setup(s => s.ExportAllToExcelAsync())
                                .ReturnsAsync(fakeStream);

            // Act
            var result = await _controller.ExportToExcel();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(Formats.ExcelFileContentType, fileResult.ContentType);
        }
    }
}
