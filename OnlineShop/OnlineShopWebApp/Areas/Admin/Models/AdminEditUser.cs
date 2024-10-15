﻿using OnlineShopWebApp.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AdminEditUser
    {
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }
        public string? Name { get; set; }

        [PhoneValidation()]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public Guid RoleId { get; set; }
    }
}