using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public ProductCategoriesViewModel Category { get; init; }
        public List<ImageViewModel> Images { get; init; } = [];
        public Dictionary<string, string> Specifications { get; set; }
        public long Article => GetArticle();

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
