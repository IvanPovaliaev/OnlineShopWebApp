﻿using OnlineShopWebApp.Models.Abstractions;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AdminRegisterViewModel : RegisterViewModel
    {
        public string RoleId { get; set; }
    }
}
