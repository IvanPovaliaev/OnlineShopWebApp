using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public ProductCategories Category { get; set; }

        //Необходимые характеристики будут определяться на фронте в зависимости от категории
        public Dictionary<string, string> Specifications { get; set; }

        public Product()
        {
            Specifications = [];
        }

        public Product(int id, string name, decimal cost, string description, ProductCategories category) : this()
        {
            Id = id;
            Name = name;
            Cost = cost;
            Description = description;
            Category = category;
        }
    }
}
