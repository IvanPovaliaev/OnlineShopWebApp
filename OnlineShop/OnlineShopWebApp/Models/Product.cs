using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(80, MinimumLength = 6, ErrorMessage = "Наименование продукта должно содержать от {2} до {1} символов.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Range(10, 10000000, ErrorMessage = "Цена должна быть от {1} до {2} руб.")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public ProductCategories Category { get; set; }
        public string? ImageUrl { get; set; }
        public long Article => GetArticle();
        public Dictionary<string, string> Specifications { get; set; }

        public Product()
        {
            Id = Guid.NewGuid();
        }

        public Product(string name, decimal cost, string description, ProductCategories category, string? imageUrl = null) : this()
        {
            Name = name;
            Cost = cost;
            Description = description;
            Category = category;
            Specifications = [];
            ImageUrl = imageUrl;
        }

        /// <summary>
        /// Get Article
        /// </summary>
        /// <returns>positive 64-bit integer</returns>
        private long GetArticle()
        {
            var bytes = Id.ToByteArray();
            var article = BitConverter.ToInt64(bytes, 0);
            return Math.Abs(article);
        }
    }
}
