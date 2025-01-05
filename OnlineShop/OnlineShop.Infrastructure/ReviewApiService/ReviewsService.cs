using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Application.Interfaces;
using OnlineShop.Infrastructure.ReviewApiService.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.ReviewApiService
{
    public class ReviewsService : IReviewsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReviewsService> _logger;
        private readonly IAccountsService _accountsService;
        private readonly IProductsService _productService;
        private readonly ReviewTokenStorage _tokenStorage;
        private readonly ReviewsSettings _reviewsSettings;

        public ReviewsService(IHttpClientFactory httpClientFactory, ILogger<ReviewsService> logger, IAccountsService accountsService, IProductsService productsService, ReviewTokenStorage tokenStorage, IOptionsSnapshot<ReviewsSettings> reviewsSettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _accountsService = accountsService;
            _productService = productsService;
            _tokenStorage = tokenStorage;
            _reviewsSettings = reviewsSettings.Value;
        }

        public async Task<List<ReviewDTO>> GetReviewsByProductIdAsync(Guid productId)
        {
            var client = _httpClientFactory.CreateClient("ReviewsService");

            try
            {
                var token = await GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/Review/GetAllByProductId?productId={productId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ReviewDTO>>(content)!;
                }

                _logger.LogError($"Failed to fetch reviews. Status code: {response.StatusCode}");
                return [];
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching reviews: {ex.Message}");
                return [];
            }
        }

        public async Task<bool> AddReviewAsync(AddReviewViewModel review)
        {
            var client = _httpClientFactory.CreateClient("ReviewsService");

            try
            {
                var token = await GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(review), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/Review", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Review successfully added.");
                    return true;
                }

                _logger.LogError($"Failed to add review. Status code: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while adding review: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsNewValidAsync(ModelStateDictionary modelState, AddReviewViewModel newReview)
        {
            var isUserExist = await _accountsService.IsUserExistAsync(newReview.UserId);
            if (!isUserExist)
            {
                modelState.AddModelError(string.Empty, $"Пользователь с id {newReview.UserId} не найден");
            }

            var product = await _productService.GetAsync(newReview.ProductId);

            if (product is null)
            {
                modelState.AddModelError(string.Empty, $"Продукт с id {newReview.ProductId} не найден");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Retrieves an authentication token for accessing the Reviews API.
        /// </summary>
        /// <returns>
        /// Returns a JWT token as a string to be used in API authorization headers.
        /// If the authentication request fails, an empty string is returned.
        /// </returns>
        private async Task<string> GetTokenAsync()
        {
            if (_tokenStorage.Expiration > DateTime.UtcNow)
            {
                return _tokenStorage.Token!;
            }

            var client = _httpClientFactory.CreateClient("ReviewsService");
            var loginDTO = new
            {
                userName = _reviewsSettings.Login,
                password = _reviewsSettings.Password
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(loginDTO), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Authentication/login", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<JWTTokenResponse>(responseContent);

            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenResponse.Token);
            var expirationClaim = token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim!)).UtcDateTime;

            _tokenStorage.Expiration = expirationDate;
            _tokenStorage.Token = tokenResponse.Token;

            return _tokenStorage.Token!;
        }
    }
}
