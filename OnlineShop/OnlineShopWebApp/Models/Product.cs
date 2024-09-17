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
        public long Article
        {
            get => GetArticle();
        }

        //Необходимые характеристики будут определяться на фронте в зависимости от категории
        public Dictionary<string, string> Specifications { get; set; }

        public Product(string name, decimal cost, string description, ProductCategories category)
        {
            Id = Guid.NewGuid();
            Name = name;
            Cost = cost;
            Description = description;
            Category = category;
            Specifications = new Dictionary<string, string>(); ;
        }

        /// <summary>
        /// Get Article for current product
        /// </summary>
        /// <returns>positive 64-bit ineger</returns>
        private long GetArticle()
        {
            var bytes = Id.ToByteArray();
            var article = BitConverter.ToInt64(bytes, 0);
            return Math.Abs(article);
        }
    }
}
