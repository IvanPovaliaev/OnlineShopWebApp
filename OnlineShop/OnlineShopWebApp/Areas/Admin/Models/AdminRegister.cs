using OnlineShopWebApp.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AdminRegister : Register
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public Guid RoleId { get; set; }
    }
}
