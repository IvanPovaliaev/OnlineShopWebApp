using OnlineShopWebApp.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AdminEditUserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }
        public string? Name { get; set; }

        [PhoneValidation()]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string RoleName { get; set; }
    }
}
