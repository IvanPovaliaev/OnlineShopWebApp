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

        [Required(ErrorMessage = "Не указана оценка")]
        [Range(0, 5, ErrorMessage = "Оценка должна быть от {1} до {2} руб.")]
        public int Grade { get; init; }
    }
}
