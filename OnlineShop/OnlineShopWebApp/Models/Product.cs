using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public ProductCategories Category { get; set; }
        public string? ImageUrl { get; set; }
        public long Article => GetArticle();

        //Необходимые характеристики будут определяться на фронте в зависимости от категории
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
