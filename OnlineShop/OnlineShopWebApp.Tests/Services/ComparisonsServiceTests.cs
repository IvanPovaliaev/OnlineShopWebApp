using AutoMapper;
using Bogus;
using LinqSpecs;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Services;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
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
        private readonly Mock<IProductsService> _productsServiceMock;
        private readonly ComparisonsService _comparisonsService;
        private readonly IMapper _mapper;

        private readonly string _userId;
        private readonly List<ComparisonProduct> _fakeComparisonProducts;
        private readonly Faker<Product> _productsFaker;

        public ComparisonsServiceTests(Mock<IComparisonsRepository> comparisonsRepositoryMock, Mock<IProductsService> productsServiceMock, IMapper mapper, FakerProvider fakerProvider)
        {
            _comparisonsRepositoryMock = comparisonsRepositoryMock;
            _productsServiceMock = productsServiceMock;

            _mapper = mapper;

            _comparisonsService = new ComparisonsService(
                _comparisonsRepositoryMock.Object,
                _mapper,
                _productsServiceMock.Object);

            _userId = fakerProvider.UserId;
            _productsFaker = fakerProvider.ProductFaker;
            _fakeComparisonProducts = fakerProvider.FakeComparisonProducts;
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasComparisons_ReturnsUserComparisons()
        {
            // Arrange
            var expectedCount = _fakeComparisonProducts.Count;
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<ComparisonProduct>>()))
                                      .ReturnsAsync(_fakeComparisonProducts);

            // Act
            var result = await _comparisonsService.GetAllAsync(_userId);

            // Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, comparison => Assert.Equal(_userId, comparison.UserId));
        }

        [Fact]
        public async Task GetAllAsync_WhenUserHasNoComparisons_ReturnsEmpty()
        {
            // Arrange
            var anotherUserId = Guid.NewGuid().ToString();
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<ComparisonProduct>>()))
                                      .ReturnsAsync([]);

            // Act
            var result = await _comparisonsService.GetAllAsync(anotherUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupsAsync_WhenUserHasComparisons_ReturnsProductsGroupedByCategory()
        {
            // Arrange
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<ComparisonProduct>>()))
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
            var anotherUserId = Guid.NewGuid().ToString();
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<ComparisonProduct>>()))
                                      .ReturnsAsync([]);

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
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<ComparisonProduct>>()))
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
            _comparisonsRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Specification<ComparisonProduct>>()))
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
