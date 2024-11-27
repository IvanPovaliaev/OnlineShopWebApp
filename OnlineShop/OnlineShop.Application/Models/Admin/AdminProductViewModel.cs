using Newtonsoft.Json;
using OnlineShop.Application.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Admin
{
    public abstract class AdminProductViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(80, MinimumLength = 6, ErrorMessage = "Наименование продукта должно содержать от {2} до {1} символов.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Range(10, 10000000, ErrorMessage = "Цена должна быть от {1} до {2} руб.")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public ProductCategoriesViewModel Category { get; set; }
        public List<ImageViewModel> Images { get; set; } = [];

        [SpecificationsValidation()]
        public Dictionary<string, string> Specifications { get; set; }
    }
}
