using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Infrastructure.ReviewApiService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.ReviewApiService
{
    public interface IReviewsService
    {
        /// <summary>
        /// Retrieves a list of reviews for a specific product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns>List of <see cref="ReviewDTO"/> representing the reviews for the product.</returns>
        Task<List<ReviewDTO>> GetReviewsByProductIdAsync(Guid productId);

        /// <summary>
        /// Adds a new review to the system.
        /// </summary>
        /// <param name="review"><see cref="AddReviewViewModel"/>.</param>
        /// <returns>True if the review was successfully added; otherwise false.</returns>
        Task<bool> AddReviewAsync(AddReviewViewModel review);

        /// <summary>
        /// Validates a new review before it is added to the system.
        /// </summary>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> used to record any validation errors.</param>
        /// <param name="newReview">The review data to validate, encapsulated in an <see cref="AddReviewViewModel"/>.</param>
        /// <returns>True if the review is valid; otherwise, false.</returns>
        Task<bool> IsNewValidAsync(ModelStateDictionary modelState, AddReviewViewModel newReview);
    }
}
