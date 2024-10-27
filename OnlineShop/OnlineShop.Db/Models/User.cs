using System;

namespace OnlineShop.Db.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public Role Role { get; set; }
    }
}
