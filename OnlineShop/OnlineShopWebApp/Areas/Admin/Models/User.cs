using System;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public Role Role { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
