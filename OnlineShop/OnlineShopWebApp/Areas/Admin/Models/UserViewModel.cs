using System;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class UserViewModel
    {
        public Guid Id { get; init; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public RoleViewModel Role { get; set; }
    }
}
