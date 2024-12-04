using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Infrastructure.ApiServices
{
    public class AddReviewViewModel
    {
        [Required(ErrorMessage = "Не указан ProductId")]
        public Guid ProductId { get; init; }

        [Required(ErrorMessage = "Не указан UserId")]
        public string UserId { get; init; }

        public string? Text { get; init; }

        [Required(ErrorMessage = "Не указан Grade")]
        [Range(0, 5, ErrorMessage = "Цена должна быть от {1} до {2} руб.")]
        public int Grade { get; init; }
    }
}
