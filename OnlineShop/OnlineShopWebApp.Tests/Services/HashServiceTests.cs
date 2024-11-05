using OnlineShopWebApp.Services;
using System;
using Xunit;

namespace OnlineShopWebApp.Tests.Services
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
            var input1 = "password1";
            var input2 = "password2";

            var hash1 = _hashService.GenerateHash(input1);
            var hash2 = _hashService.GenerateHash(input2);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GenerateHash_WhenGivenSameInputsWithoutSameSalt_GeneratesDifferentHashes()
        {
            var input = "password1";

            var hash1 = _hashService.GenerateHash(input);
            var hash2 = _hashService.GenerateHash(input);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GenerateHash_WhenGivenSameSalt_GeneratesSameHashes()
        {
            var password = "password";
            var salt = Convert.FromBase64String(_hashService.GenerateHash(password).Split(':')[1]);

            var hash1 = _hashService.GenerateHash(password, salt);
            var hash2 = _hashService.GenerateHash(password, salt);

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void IsEquals_WhenHashMatches_ReturnsTrue()
        {
            var password = "mypassword";
            var hash = _hashService.GenerateHash(password);

            var result = _hashService.IsEquals(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void IsEquals_WhenHashDoesNotMatch_ReturnsFalse()
        {
            var password = "password1";
            var incorrectPassword = "password2";
            var hash = _hashService.GenerateHash(password);

            var result = _hashService.IsEquals(incorrectPassword, hash);

            Assert.False(result);
        }
    }
}
