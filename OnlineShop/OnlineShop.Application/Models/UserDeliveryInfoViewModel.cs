using OnlineShop.Application.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models
{
    public class UserDeliveryInfoViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string City { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Address { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Значение должно быть больше 0")]
        public int? Entrance { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Значение должно быть больше 0")]
        public int? Floor { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Значение должно быть больше 0")]
        public int? Apartment { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"\d{6}", ErrorMessage = "Формат индекса: 000000")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^[А-ЯЁA-Z][а-яёa-zA-Z'-]+ [А-ЯЁA-Z][а-яёa-zA-Z'-]+(\s[А-ЯЁA-Z][а-яёa-zA-Z'-]+)?$", ErrorMessage = "Введите полное имя в виде: Фамилия Имя Отчество (при наличии)")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [PhoneValidation()]
        public string Phone { get; set; }

        [PhoneValidation()]
        public string? ReservePhone { get; set; }

        [StringLength(300, MinimumLength = 10, ErrorMessage = "Количество символов должно быть от {2} до {1}")]
        public string? AdditionalInfo { get; set; }
    }
}
