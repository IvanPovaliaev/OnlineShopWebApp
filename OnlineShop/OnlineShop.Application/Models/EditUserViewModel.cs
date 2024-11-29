using Microsoft.AspNetCore.Http;
using OnlineShop.Application.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models
{
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "Не указан UserId")]
        public required string Id { get; init; }

        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }
        public string? FullName { get; set; }

        [PhoneValidation()]
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public IFormFile? UploadedImage { get; init; }
    }
}
