using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class CookieCartViewModel
    {
        public Guid Id { get; init; }
        public List<CookieCartPositionViewModel> Positions { get; set; } = [];

        public CookieCartViewModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
