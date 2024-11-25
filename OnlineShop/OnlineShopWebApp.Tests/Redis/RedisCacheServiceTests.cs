using Moq;
using OnlineShopWebApp.Redis;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShopWebApp.Tests.Redis
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly RedisCacheService _service;
        private readonly string _hashKey;
        private readonly string _fieldKey;
        private readonly string _value;

        public RedisCacheServiceTests()
        {
            _redisMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();

            _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                      .Returns(_databaseMock.Object);

            _service = new RedisCacheService(_redisMock.Object);
            _hashKey = "testHash";
            _fieldKey = "testField";
            _value = "testValue";
        }

        [Fact]
        public async Task SetHashFieldAsync_WhenRedisAvailable_CallHashSetAsync()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected).Returns(true);

            // Act
            await _service.SetHashFieldAsync(_hashKey, _fieldKey, _value);

            // Assert
            _databaseMock.Verify(db => db.HashSetAsync(_hashKey, _fieldKey, _value, It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task SetHashFieldAsync_WhenRedisUnavailable_NotCallHashSetAsync()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(false);

            // Act
            await _service.SetHashFieldAsync(_hashKey, _fieldKey, _value);

            // Assert
            _databaseMock.Verify(db => db.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
        }

        [Fact]
        public async Task SetHashFieldAsync_RedisConnectionException_HandleRedisConnectionException()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected).Returns(true);

            _databaseMock.Setup(db => db.HashSetAsync(_hashKey, _fieldKey, _value, It.IsAny<When>(), It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.ConnectionDisposed, "Test Exception"));

            // Act
            await _service.SetHashFieldAsync(_hashKey, _fieldKey, _value);

            // Assert
            _databaseMock.Verify(db => db.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task SetHashFieldsAsync_WhenRedisAvailable_CallHashSetAsync()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(true);
            var entries = new Dictionary<string, string>
            {
                { "field1", "value1" },
                { "field2", "value2" }
            };

            var expectedEntries = entries.Select(e => new HashEntry(e.Key, e.Value))
                                         .ToArray();

            // Act
            await _service.SetHashFieldsAsync(_hashKey, entries);

            // Assert
            _databaseMock.Verify(db => db.HashSetAsync(_hashKey, It.Is<HashEntry[]>(h => h.SequenceEqual(expectedEntries)), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task SetHashFieldsAsync_WhenRedisUnavailable_NotCallHashSetAsync()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected).Returns(false);

            var entries = new Dictionary<string, string>
            {
                { "field1", "value1" },
                { "field2", "value2" }
            };

            // Act
            await _service.SetHashFieldsAsync(_hashKey, entries);

            // Assert
            _databaseMock.Verify(db => db.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()), Times.Never);
        }

        [Fact]
        public async Task SetHashFieldsAsync_RedisConnectionException_HandleException()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected).Returns(true);

            var entries = new Dictionary<string, string>
            {
                { "field1", "value1" },
                { "field2", "value2" }
            };

            _databaseMock.Setup(db => db.HashSetAsync(_hashKey, It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.ConnectionDisposed, "Test Exception"));

            // Act
            await _service.SetHashFieldsAsync(_hashKey, entries);

            // Assert
            _databaseMock.Verify(db => db.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task TryGetHashFieldAsync_WhenFieldExists_ReturnValue()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected).Returns(true);
            var expectedValue = _value;

            _databaseMock.Setup(db => db.HashGetAsync(_hashKey, _fieldKey, It.IsAny<CommandFlags>()))
                         .ReturnsAsync(expectedValue);

            // Act
            var result = await _service.TryGetHashFieldAsync(_hashKey, _fieldKey);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async Task TryGetHashFieldAsync_WhenRedisUnavailable_ReturnNull()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(false);

            // Act
            var result = await _service.TryGetHashFieldAsync(_hashKey, _fieldKey);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task TryGetHashFieldAsync_RedisConnectionException_HandleException()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(true);

            _databaseMock.Setup(db => db.HashGetAsync(_hashKey, _fieldKey, It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.ConnectionDisposed, "Test Exception"));

            // Act
            var result = await _service.TryGetHashFieldAsync(_hashKey, _fieldKey);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetAllValuesAsync_WhenValuesExist_ReturnsAllValues()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(true);

            var redisValues = new RedisValue[] { "value1", "value2", "value3" };

            _databaseMock.Setup(db => db.HashValuesAsync(_hashKey, It.IsAny<CommandFlags>()))
                         .ReturnsAsync(redisValues);

            // Act
            var result = await _service.GetAllValuesAsync(_hashKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(redisValues.Select(v => v.ToString()).ToList(), result);
        }

        [Fact]
        public async Task GetAllValuesAsync_WhenRedisUnavailable_ReturnsNull()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(false);

            // Act
            var result = await _service.GetAllValuesAsync(_hashKey);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllValuesAsync_RedisConnectionException_HandleExceptionAndReturnNull()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(true);

            _databaseMock.Setup(db => db.HashValuesAsync(_hashKey, It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.ConnectionDisposed, "Test Exception"));

            // Act
            var result = await _service.GetAllValuesAsync(_hashKey);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveHashFieldAsync_WhenRedisAvailable_CallHashDeleteAsync()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(true);

            // Act
            await _service.RemoveHashFieldAsync(_hashKey, _fieldKey);

            // Assert
            _databaseMock.Verify(db => db.HashDeleteAsync(_hashKey, _fieldKey, It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task RemoveHashFieldAsync_WhenRedisUnavailable_NotCallHashDeleteAsync()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(false);

            // Act
            await _service.RemoveHashFieldAsync(_hashKey, _fieldKey);

            // Assert
            _databaseMock.Verify(db => db.HashDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Never);
        }

        [Fact]
        public async Task RemoveHashFieldAsync_RedisConnectionException_HandleException()
        {
            // Arrange
            _redisMock.Setup(r => r.IsConnected)
                      .Returns(true);

            _databaseMock.Setup(db => db.HashDeleteAsync(_hashKey, _fieldKey, It.IsAny<CommandFlags>()))
                         .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.ConnectionDisposed, "Test Exception"));

            // Act
            await _service.RemoveHashFieldAsync(_hashKey, _fieldKey);

            // Assert
            _databaseMock.Verify(db => db.HashDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}
