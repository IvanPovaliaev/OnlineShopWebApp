using OnlineShopWebApp.Models.Abstractions;
using System;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AdminRegister : Register
    {
        public Guid RoleId { get; set; }
    }
}
