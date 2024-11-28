using OnlineShop.Application.Services;
using System;
using Xunit;

namespace OnlineShopWebApp.Tests.Application.Services
{
    public class HashServiceTests
    {
        private readonly HashService _hashService;

        public HashServiceTests()
        {
            _hashService = new HashService();
        }

        [Fact]
        public void GenerateHash_WhenGivenDifferentInputs_GeneratesDifferentHashes()
        {
            // Arrange
            var input1 = "password1";
            var input2 = "password2";

            // Act
            var hash1 = _hashService.GenerateHash(input1);
            var hash2 = _hashService.GenerateHash(input2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GenerateHash_WhenGivenSameInputsWithoutSameSalt_GeneratesDifferentHashes()
        {
            // Arrange
            var input = "password1";

            // Act
            var hash1 = _hashService.GenerateHash(input);
            var hash2 = _hashService.GenerateHash(input);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GenerateHash_WhenGivenSameSalt_GeneratesSameHashes()
        {
            // Arrange
            var password = "password";
            var salt = Convert.FromBase64String(_hashService.GenerateHash(password).Split(':')[1]);

            // Act
            var hash1 = _hashService.GenerateHash(password, salt);
            var hash2 = _hashService.GenerateHash(password, salt);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void IsEquals_WhenHashMatches_ReturnsTrue()
        {
            // Arrange
            var password = "mypassword";
            var hash = _hashService.GenerateHash(password);

            // Act
            var result = _hashService.IsEquals(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEquals_WhenHashDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var password = "password1";
            var incorrectPassword = "password2";
            var hash = _hashService.GenerateHash(password);

            // Act
            var result = _hashService.IsEquals(incorrectPassword, hash);

            // Assert
            Assert.False(result);
        }
    }
}
