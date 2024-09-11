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

        //Необходимые характеристики будут определяться на фронте в зависимости от категории
        public Dictionary<string, string> Specifications { get; set; }

        public Product()
        {
            Id = Guid.NewGuid();
            Specifications = new Dictionary<string, string>();
        }

        public Product(string name, decimal cost, string description, ProductCategories category) : this()
        {
            Name = name;
            Cost = cost;
            Description = description;
            Category = category;
        }

        public override string ToString() => $"{Id}\n{Name}\n{Cost}";
    }
}
