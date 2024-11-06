using AutoMapper;
using Bogus;
using Moq;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Services;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
{
    public class ComparisonsServiceTests
    {
        private readonly Mock<IComparisonsRepository> _comparisonsRepositoryMock;
        private readonly Mock<ProductsService> _productsServiceMock;
        private readonly ComparisonsService _comparisonsService;
        private readonly IMapper _mapper;

        private const int ProductsCount = 10;
        private readonly Guid _userId;
        private readonly List<ComparisonProduct> _fakeComparisonProducts;
        private readonly Faker<Product> _productsFaker;

        public ComparisonsServiceTests()
        {
            _comparisonsRepositoryMock = new Mock<IComparisonsRepository>();
            _productsServiceMock = new Mock<ProductsService>(null!, null!, null!, null!);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TestMappingProfile>());
            _mapper = config.CreateMapper();

            _comparisonsService = new ComparisonsService(
                _comparisonsRepositoryMock.Object,
                _mapper,
                _productsServiceMock.Object);

            _userId = Guid.NewGuid();

            _productsFaker = new Faker<Product>()
                    .RuleFor(p => p.Id, f => Guid.NewGuid())
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Cost, f => f.Finance.Amount())
                    .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                    .RuleFor(p => p.Category, f => f.PickRandom<ProductCategories>())
                    .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl());

            _fakeComparisonProducts = new Faker<ComparisonProduct>()
                    .RuleFor(fp => fp.Id, f => Guid.NewGuid())
                    .RuleFor(fp => fp.UserId, f => _userId)
                    .RuleFor(fp => fp.Product, f => _productsFaker.Generate())
                    .Generate(ProductsCount);
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasComparisons_ReturnsUserComparisons()
        {
            // Arrange
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                      .ReturnsAsync(_fakeComparisonProducts);

            // Act
            var result = await _comparisonsService.GetAllAsync(_userId);

            // Assert
            Assert.Equal(ProductsCount, result.Count);
            Assert.All(result, comparison => Assert.Equal(_userId, comparison.UserId));
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasNoComparisons_ReturnsEmpty()
        {
            // Arrange
            var anotherUserId = Guid.NewGuid();
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                      .ReturnsAsync(_fakeComparisonProducts);

            // Act
            var result = await _comparisonsService.GetAllAsync(anotherUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupsAsync_WhenUserHasComparisons_ReturnsProductsGroupedByCategory()
        {
            // Arrange
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                      .ReturnsAsync(_fakeComparisonProducts);

            // Act
            var result = await _comparisonsService.GetGroupsAsync(_userId);

            // Assert
            var categoryCount = _fakeComparisonProducts.Select(p => p.Product.Category).Distinct().Count();
            Assert.Equal(categoryCount, result.Count);

            var resultComparisonCount = result.Sum(category => category.Count());
            Assert.Equal(_fakeComparisonProducts.Count, resultComparisonCount);
            foreach (var category in result)
            {
                Assert.All(category, comparison => Assert.Equal(category.Key, comparison.Product.Category));
            }
        }

        [Fact]
        public async Task GetGroupsAsync_WhenUserHasNoComparisons_ReturnsEmpty()
        {
            // Arrange
            var anotherUserId = Guid.NewGuid();
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                      .ReturnsAsync(_fakeComparisonProducts);

            // Act
            var result = await _comparisonsService.GetGroupsAsync(anotherUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_WhenProductIsNotInComparisons_AddsProductToComparison()
        {
            // Arrange
            var fakeProduct = _productsFaker.Generate();
            _productsServiceMock.Setup(service => service.GetAsync(fakeProduct.Id))
                                .ReturnsAsync(fakeProduct);
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeComparisonProducts);

            // Act
            await _comparisonsService.CreateAsync(fakeProduct.Id, _userId);

            // Assert
            _comparisonsRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<ComparisonProduct>(c => c.Product == fakeProduct && c.UserId == _userId)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenProductInFavorites_NotAddProductToComparisons()
        {
            // Arrange
            var fakeProduct = _fakeComparisonProducts.First().Product;
            _productsServiceMock.Setup(service => service.GetAsync(fakeProduct.Id))
                                .ReturnsAsync(fakeProduct);
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(_fakeComparisonProducts);

            // Act
            await _comparisonsService.CreateAsync(fakeProduct.Id, _userId);

            // Assert
            _comparisonsRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<ComparisonProduct>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_DeletesComparisonProduct()
        {
            // Arrange
            var fakeComparison = _fakeComparisonProducts.First();

            // Act
            await _comparisonsService.DeleteAsync(fakeComparison.Id);

            // Assert
            _comparisonsRepositoryMock.Verify(repo => repo.DeleteAsync(fakeComparison.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAllAsync_WhenCalled_DeletesAllComparisonsForUser()
        {
            // Act
            await _comparisonsService.DeleteAllAsync(_userId);

            // Assert
            _comparisonsRepositoryMock.Verify(repo => repo.DeleteAllAsync(_userId), Times.Once);
        }
    }
}
