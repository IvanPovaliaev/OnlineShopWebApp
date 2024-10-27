using OnlineShopWebApp.Models.Abstractions;
using System;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AdminRegisterViewModel : RegisterViewModel
    {
        public Guid RoleId { get; set; }
    }
}
