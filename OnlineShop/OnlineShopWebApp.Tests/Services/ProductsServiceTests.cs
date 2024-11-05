using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
{
    public class ProductsServiceTests
    {
        private readonly Mock<IProductsRepository> _productsRepositoryMock;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductsService _productsService;

        private const int ProductsCount = 10;
        private readonly List<Product> _fakeProducts;

        public ProductsServiceTests()
        {
            _productsRepositoryMock = new Mock<IProductsRepository>();
            _excelServiceMock = new Mock<IExcelService>();
            _mapperMock = InitializeMapperMock();

            var rules = new List<IProductSpecificationsRules>();

            _productsService = new ProductsService(
                _productsRepositoryMock.Object,
                _mapperMock.Object,
                _excelServiceMock.Object,
                rules
            );

            _fakeProducts = InitializeFakeProducts(ProductsCount);
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnAllProducts()
        {
            // Arrange
            _productsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                   .ReturnsAsync(_fakeProducts);

            // Act
            var result = await _productsService.GetAllAsync();

            // Assert
            Assert.Equal(_fakeProducts.Count, result.Count);
            _productsRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithCategory_ReturnsFilteredProducts()
        {
            // Arrange
            var category = new Faker().PickRandom<ProductCategoriesViewModel>();

            _productsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                   .ReturnsAsync(_fakeProducts);

            // Act
            var result = await _productsService.GetAllAsync(category);

            // Assert
            Assert.All(result, p => Assert.Equal(category, p.Category));
            _productsRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllFromSearchAsync_WithValidQuery_ReturnsMatchingProducts()
        {
            // Arrange
            var query = "testQuery123!23112";
            var productCount = new Random().Next(1, ProductsCount);

            for (int i = 0; i < productCount; i++)
            {
                var addToEnd = new Random().Next(2) == 0;

                if (addToEnd)
                {
                    _fakeProducts[i].Name += query;
                    continue;
                }
                _fakeProducts[i].Name = _fakeProducts[i].Name.Insert(0, query);
            }

            _productsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_fakeProducts);

            // Act
            var result = await _productsService.GetAllFromSearchAsync(query);

            // Assert
            Assert.Contains(result, p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
            _productsRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenProductExists_ReturnProduct()
        {
            // Arrange
            var expectedProduct = _fakeProducts.First();

            _productsRepositoryMock.Setup(repo => repo.GetAsync(expectedProduct.Id))
                                   .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productsService.GetAsync(expectedProduct.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct, result);
            _productsRepositoryMock.Verify(repo => repo.GetAsync(expectedProduct.Id), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenProductDoesNotExist_ReturnNull()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productsRepositoryMock.Setup(repo => repo.GetAsync(productId))
                                   .ReturnsAsync((Product)null!);

            // Act
            var result = await _productsService.GetAsync(productId);

            // Assert
            Assert.Null(result);
            _productsRepositoryMock.Verify(repo => repo.GetAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetViewModelAsync_WhenProductExists_ReturnsMappedProductViewModel()
        {
            // Arrange
            var fakeProduct = _fakeProducts.First();

            _productsRepositoryMock.Setup(repo => repo.GetAsync(fakeProduct.Id))
                                   .ReturnsAsync(fakeProduct);

            // Act
            var result = await _productsService.GetViewModelAsync(fakeProduct.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeProduct.Id, result.Id);
            Assert.Equal(fakeProduct.Name, result.Name);
            Assert.Equal(fakeProduct.Cost, result.Cost);
            Assert.Equal(fakeProduct.Description, result.Description);
            Assert.Equal(fakeProduct.ImageUrl, result.ImageUrl);
            Assert.Equal((ProductCategoriesViewModel)fakeProduct.Category, result.Category);
            _productsRepositoryMock.Verify(repo => repo.GetAsync(fakeProduct.Id), Times.Once);
        }

        [Fact]
        public async Task GetViewModelAsync_WhenProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productsRepositoryMock.Setup(repo => repo.GetAsync(productId))
                                   .ReturnsAsync((Product)null!);

            // Act
            var result = await _productsService.GetViewModelAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEditProductAsync_WhenProductExist_ReturnsMappedEditProductViewModel()
        {
            // Arrange
            var fakeProduct = _fakeProducts.First();

            _productsRepositoryMock.Setup(repo => repo.GetAsync(fakeProduct.Id))
                                   .ReturnsAsync(fakeProduct);

            _mapperMock.Setup(m => m.Map<EditProductViewModel>(fakeProduct)).Returns(new EditProductViewModel
            {
                Id = fakeProduct.Id,
                Name = fakeProduct.Name,
                Cost = fakeProduct.Cost,
                Description = fakeProduct.Description,
                ImageUrl = fakeProduct.ImageUrl,
                Category = (ProductCategoriesViewModel)fakeProduct.Category
            });

            // Act
            var result = await _productsService.GetEditProductAsync(fakeProduct.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeProduct.Id, result.Id);
            Assert.Equal(fakeProduct.Name, result.Name);
            Assert.Equal(fakeProduct.Cost, result.Cost);
            Assert.Equal(fakeProduct.Description, result.Description);
            Assert.Equal(fakeProduct.ImageUrl, result.ImageUrl);
            Assert.Equal((ProductCategoriesViewModel)fakeProduct.Category, result.Category);
            _productsRepositoryMock.Verify(repo => repo.GetAsync(fakeProduct.Id), Times.Once);
        }

        [Fact]
        public async Task GetEditProductAsync_WhenProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productsRepositoryMock.Setup(repo => repo.GetAsync(productId))
                                   .ReturnsAsync((Product)null!);

            _mapperMock.Setup(m => m.Map<EditProductViewModel>(null))
                       .Returns((EditProductViewModel)null!);

            // Act
            var result = await _productsService.GetViewModelAsync(productId);

            // Assert
            Assert.Null(result);
            _productsRepositoryMock.Verify(repo => repo.GetAsync(productId), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProductToRepository()
        {
            // Arrange
            var newProduct = new AddProductViewModel
            {
                Name = "SSD 1Tb Kingston NV2 (SNV2S/1000G)",
                Cost = 100,
                Description = "New Product Description",
                Specifications = new Dictionary<string, string>
                {
                    { "ManufacturerCode", "SNV2S/1000G" }
                }
            };

            var mappedProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = newProduct.Name,
                Cost = newProduct.Cost,
                Description = newProduct.Description,
                SpecificationsJson = System.Text.Json.JsonSerializer.Serialize(newProduct.Specifications)
            };

            _mapperMock.Setup(m => m.Map<Product>(newProduct))
                       .Returns(mappedProduct);
            _productsRepositoryMock.Setup(repo => repo.AddAsync(mappedProduct))
                                   .Returns(Task.CompletedTask);

            // Act
            await _productsService.AddAsync(newProduct);

            // Assert
            _productsRepositoryMock.Verify(repo => repo.AddAsync(mappedProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCalled_UpdateProductInRepository()
        {
            // Arrange
            var product = new EditProductViewModel
            {
                Id = Guid.NewGuid(),
                Name = "SSD 1Tb Kingston NV2 (SNV2S/1000G)",
                Cost = 200,
                Description = "Updated Description",
                Specifications = new Dictionary<string, string>
                {
                    { "ManufacturerCode", "SNV2S/1000G" }
                }
            };

            var mappedProduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Cost = product.Cost,
                Description = product.Description,
                SpecificationsJson = System.Text.Json.JsonSerializer.Serialize(product.Specifications)
            };

            _mapperMock.Setup(m => m.Map<Product>(product))
                       .Returns(mappedProduct);
            _productsRepositoryMock.Setup(repo => repo.UpdateAsync(mappedProduct))
                                   .Returns(Task.CompletedTask);

            // Act
            await _productsService.UpdateAsync(product);

            // Assert
            _productsRepositoryMock.Verify(repo => repo.UpdateAsync(mappedProduct), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_RemoveProductFromRepository()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productsRepositoryMock.Setup(repo => repo.DeleteAsync(productId))
                                   .Returns(Task.CompletedTask);

            // Act
            await _productsService.DeleteAsync(productId);

            // Assert
            _productsRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
        }

        [Fact]
        public async Task IsUpdateValidAsync_SameCategory_ModelStateIsValid()
        {
            // Arrange
            var fakeProduct = _fakeProducts.First();
            var modelState = new ModelStateDictionary();
            var editProduct = new EditProductViewModel
            {
                Id = fakeProduct.Id,
                Name = fakeProduct.Name,
                Cost = fakeProduct.Cost,
                Description = fakeProduct.Description,
                ImageUrl = fakeProduct.ImageUrl,
                Category = (ProductCategoriesViewModel)fakeProduct.Category
            };

            _productsRepositoryMock.Setup(repo => repo.GetAsync(fakeProduct.Id))
                                   .ReturnsAsync(fakeProduct);

            // Act
            var result = await _productsService.IsUpdateValidAsync(modelState, editProduct);

            // Assert
            Assert.True(result);
            Assert.Equal(0, modelState.ErrorCount);
            _productsRepositoryMock.Verify(repo => repo.GetAsync(fakeProduct.Id), Times.Once);
        }

        [Fact]
        public async Task IsUpdateValidAsync_DifferentCategory_AddsModelError()
        {
            // Arrange
            var fakeProduct = _fakeProducts.First();
            var modelState = new ModelStateDictionary();

            var categoriesExceptTarget = Enum.GetValues(typeof(ProductCategories))
                                                    .Cast<ProductCategories>()
                                                    .Where(c => c != fakeProduct.Category)
                                                    .Cast<ProductCategoriesViewModel>();

            var editProduct = new EditProductViewModel
            {
                Id = fakeProduct.Id,
                Name = fakeProduct.Name,
                Cost = fakeProduct.Cost,
                Description = fakeProduct.Description,
                ImageUrl = fakeProduct.ImageUrl,
                Category = new Faker().PickRandom(categoriesExceptTarget)
            };

            _productsRepositoryMock.Setup(repo => repo.GetAsync(fakeProduct.Id))
                                   .ReturnsAsync(fakeProduct);

            // Act
            var result = await _productsService.IsUpdateValidAsync(modelState, editProduct);

            // Assert
            Assert.False(result);
            Assert.Contains(modelState, m => m.Value!.Errors.Any());
            _productsRepositoryMock.Verify(repo => repo.GetAsync(fakeProduct.Id), Times.Once);
        }

        [Fact]
        public async Task ExportAllToExcelAsync_WhenCalled_ReturnsExcelFileWithProducts()
        {
            // Arrange
            var memoryStream = new MemoryStream();

            _productsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                   .ReturnsAsync(_fakeProducts);

            _excelServiceMock.Setup(service => service.ExportProducts(It.IsAny<List<ProductViewModel>>()))
                             .Returns(memoryStream);

            // Act
            var result = await _productsService.ExportAllToExcelAsync();

            // Assert
            Assert.IsType<MemoryStream>(result);
            _productsRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _excelServiceMock.Verify(service => service.ExportProducts(It.IsAny<List<ProductViewModel>>()), Times.Once);
        }

        private List<Product> InitializeFakeProducts(int count)
        {
            return new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Cost, f => f.Random.Decimal(10, 1000))
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Category, f => f.PickRandom<ProductCategories>())
                .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl())
                .Generate(count);
        }

        private Mock<IMapper> InitializeMapperMock()
        {
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ProductViewModel>(It.IsAny<Product>()))
                      .Returns((Product source) =>
                      {
                          if (source == null)
                          {
                              return null!;
                          }

                          return new ProductViewModel
                          {
                              Id = source.Id,
                              Name = source.Name,
                              Cost = source.Cost,
                              Description = source.Description,
                              ImageUrl = source.ImageUrl,
                              Category = (ProductCategoriesViewModel)source.Category
                          };
                      });
            return mapperMock;
        }
    }
}
