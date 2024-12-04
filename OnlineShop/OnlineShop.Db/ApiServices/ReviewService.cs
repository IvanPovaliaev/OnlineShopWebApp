using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.ApiServices
{
    public class ReviewService : IReviewService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IHttpClientFactory httpClientFactory, ILogger<ReviewService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<ReviewDTO>> GetReviewsByProductIdAsync(Guid productId)
        {
            var client = _httpClientFactory.CreateClient("ReviewsService");

            try
            {
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
    }
}
