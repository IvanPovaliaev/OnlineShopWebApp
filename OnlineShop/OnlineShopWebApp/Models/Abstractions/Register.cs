﻿using OnlineShopWebApp.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models.Abstractions
{
    public abstract class Register
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [PasswordValidation()]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        public string? Name { get; set; }

        [PhoneValidation()]
        public string? Phone { get; set; }
    }
}