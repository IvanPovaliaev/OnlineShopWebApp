using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.ApiServices
{
    public interface IReviewService
    {
        Task<List<ReviewDTO>> GetReviewsByProductIdAsync(Guid productId);

        Task<bool> AddReviewAsync(AddReviewViewModel review);
    }
}
