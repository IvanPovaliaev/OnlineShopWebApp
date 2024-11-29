using AutoMapper;
using Bogus;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Services;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using OnlineShopWebApp.Tests.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Application.Services
{
    public class CartsServiceTests
    {
        private readonly Mock<ICartsRepository> _cartsRepositoryMock;
        private readonly Mock<IProductsService> _productsServiceMock;
        private readonly IMapper _mapper;
        private readonly CartsService _cartsService;

        private readonly string? _userId;
        private readonly Guid _newProductId;
        private readonly Faker<Product> _productFaker;
        private readonly Cart _fakeCart;

        public CartsServiceTests(Mock<ICartsRepository> cartsRepositoryMock, Mock<IProductsService> productsServiceMock, IMapper mapper, FakerProvider fakerProvider)
        {
            _cartsRepositoryMock = cartsRepositoryMock;
            _productsServiceMock = productsServiceMock;

            _mapper = mapper;

            _cartsService = new CartsService(
                _cartsRepositoryMock.Object,
                _mapper,
                _productsServiceMock.Object);

            _userId = fakerProvider.UserId;
            _newProductId = Guid.NewGuid();
            _productFaker = fakerProvider.ProductFaker;
            _fakeCart = fakerProvider.FakeCart;
        }

        [Fact]
        public async Task GetAsync_ForSpecifiedUserId_ReturnsCart()
        {
            // Arrange
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            var result = await _cartsService.GetAsync(_userId!);

            // Assert
            Assert.Equal(_fakeCart, result);
        }

        [Fact]
        public async Task GetViewModelAsync_ForSpecifiedUserId_ReturnsMappedCartViewModel()
        {
            // Arrange
            var expectedPositionQuantity = _fakeCart.Positions.Count;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            var result = await _cartsService.GetViewModelAsync(_userId!);

            // Assert
            Assert.Equal(_fakeCart.UserId, result.UserId);
            Assert.Equal(expectedPositionQuantity, result.Positions.Count);
        }

        [Fact]
        public async Task AddAsync_IfCartExistsAndProductNotInCart_AddsProductToExistingCart()
        {
            // Arrange
            var initialPositionsCount = _fakeCart.Positions.Count;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);
            _productsServiceMock.Setup(service => service.GetAsync(_newProductId))
                                .ReturnsAsync(_productFaker.Generate());

            // Act
            await _cartsService.AddAsync(_newProductId, _userId!);

            // Assert
            Assert.Equal(initialPositionsCount + 1, _fakeCart.Positions.Count);
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Once);
        }

        [Fact]
        public async Task AddAsync_IfCartExistsAndProductInCart_IncreaseRelatedCartPosition()
        {
            // Arrange
            var fakePosition = _fakeCart.Positions.First();
            var initialPositionQuantity = fakePosition.Quantity;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);
            _productsServiceMock.Setup(service => service.GetAsync(fakePosition.Product.Id))
                                .ReturnsAsync(fakePosition.Product);

            // Act
            await _cartsService.AddAsync(fakePosition.Product.Id, _userId!);

            // Assert
            Assert.Equal(initialPositionQuantity + 1, fakePosition.Quantity);
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Once);
        }

        [Fact]
        public async Task AddAsync_IfCartDoesNotExist_CreatesNewCartAndAddsProduct()
        {
            // Arrange
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync((Cart)null!);
            _productsServiceMock.Setup(service => service.GetAsync(_newProductId))
                                .ReturnsAsync(_productFaker.Generate());

            // Act
            await _cartsService.AddAsync(_newProductId, _userId!);

            // Assert
            _cartsRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<Cart>(cart => cart.UserId == _userId && cart.Positions.Count == 1)), Times.Once);
        }

        [Fact]
        public async Task IncreasePositionAsync_ForSpecifiedPositionId_IncreasesQuantity()
        {
            // Arrange
            var fakePosition = _fakeCart.Positions.First();
            var initialPositionQuantity = fakePosition.Quantity;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId))
                                .ReturnsAsync(_fakeCart);

            // Act
            await _cartsService.IncreasePositionAsync(_userId!, fakePosition.Id);

            // Assert
            Assert.Equal(initialPositionQuantity + 1, fakePosition.Quantity);
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Once);
        }

        [Fact]
        public async Task IncreasePositionAsync_IfPositionIsNotInTargetCart_NotIncreasesQuantity()
        {
            // Arrange
            var fakePosition = new CartPosition()
            {
                Id = Guid.NewGuid(),
                Product = _productFaker.Generate(),
                Quantity = 1
            };
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            await _cartsService.IncreasePositionAsync(_userId!, fakePosition.Id);

            // Assert
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Never);
        }

        [Fact]
        public async Task DecreasePosition_IfQuantityIsOne_DeletesPosition()
        {
            // Arrange
            var position = _fakeCart.Positions.First();
            position.Quantity = 1;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            await _cartsService.DecreasePositionAsync(_userId!, position.Id);

            // Assert
            Assert.DoesNotContain(position, _fakeCart.Positions);
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Once);
        }

        [Fact]
        public async Task DecreasePosition_IfQuantityIsNotOne_DeletesPosition()
        {
            // Arrange
            var fakePosition = _fakeCart.Positions.First();
            fakePosition.Quantity = 2;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            await _cartsService.DecreasePositionAsync(_userId!, fakePosition.Id);

            // Assert
            Assert.Contains(fakePosition, _fakeCart.Positions);
            Assert.Equal(1, fakePosition.Quantity);
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Once);
        }

        [Fact]
        public async Task DecreasePosition_IfPositionIsNotInTargetCart_NotDeletesPosition()
        {
            // Arrange
            var fakePosition = new CartPosition()
            {
                Id = Guid.NewGuid(),
                Product = _productFaker.Generate(),
                Quantity = 1
            };
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            await _cartsService.DecreasePositionAsync(_userId!, fakePosition.Id);

            // Assert
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_DeletesUserCart()
        {
            // Act
            await _cartsService.DeleteAsync(_userId!);

            // Assert
            _cartsRepositoryMock.Verify(repo => repo.DeleteAsync(_userId), Times.Once);
        }

        [Fact]
        public async Task DeletePositionAsync_IfPositionInTargetCart_RemovesSpecifiedPositionFromCart()
        {
            // Arrange
            var positionId = _fakeCart.Positions.First().Id;
            var initialPositionsCount = _fakeCart.Positions.Count;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            await _cartsService.DeletePositionAsync(_userId!, positionId);

            // Assert
            Assert.DoesNotContain(_fakeCart.Positions, p => p.Id == positionId);
            Assert.Equal(initialPositionsCount - 1, _fakeCart.Positions.Count);
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Once);
        }

        [Fact]
        public async Task DeletePositionAsync_IfPositionNotInTargetCart_NotRemovesSpecifiedPositionFromCart()
        {
            // Arrange
            var positionId = Guid.NewGuid();
            var initialPositionsCount = _fakeCart.Positions.Count;
            _cartsRepositoryMock.Setup(repo => repo.GetAsync(_userId!))
                                .ReturnsAsync(_fakeCart);

            // Act
            await _cartsService.DeletePositionAsync(_userId!, positionId);

            // Assert
            Assert.DoesNotContain(_fakeCart.Positions, p => p.Id == positionId);
            Assert.Equal(initialPositionsCount, _fakeCart.Positions.Count);
            _cartsRepositoryMock.Verify(repo => repo.UpdateAsync(_fakeCart), Times.Never);
        }
    }
}
