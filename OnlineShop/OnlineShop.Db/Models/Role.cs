using System;

namespace OnlineShop.Db.Models
{
    public class Role
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public bool CanBeDeleted { get; init; } = true;
    }
}
