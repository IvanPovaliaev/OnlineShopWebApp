﻿using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Не указан Email")]
        //[EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        [RegularExpression(@"")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}
