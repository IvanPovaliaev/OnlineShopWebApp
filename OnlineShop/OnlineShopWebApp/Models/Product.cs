using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public ProductCategories Category { get; set; }

        //Необходимые характеристики будут определяться на фронте в зависимости от категории
        public Dictionary<string, string> Specifications { get; set; }

        public Product()
        {
            Id = Guid.NewGuid();
            Specifications = [];
        }

        public Product(string name, decimal cost, string description, ProductCategories category) : this()
        {
            Name = name;
            Cost = cost;
            Description = description;
            Category = category;
        }

        public string GetFullInfo()
        {
            var baseInfo = $"{Id}\n{Name}\n{Cost}\n{Description}\n{Category}";

            var specificationsInfo = Specifications.Select(spec => $"{spec.Key}: {spec.Value}");

            return $"{baseInfo}\n\nХарактеристики:\n{string.Join("\n", specificationsInfo)}";
        }

        public override string ToString() => $"{Id}\n{Name}\n{Cost}";
    }
}
