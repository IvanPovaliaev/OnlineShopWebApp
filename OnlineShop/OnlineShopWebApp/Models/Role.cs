using System;

namespace OnlineShopWebApp.Models
{
    public class Role(string name)
    {
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; } = name;
    }
}
