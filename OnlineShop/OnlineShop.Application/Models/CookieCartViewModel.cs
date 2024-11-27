using System;
using System.Collections.Generic;

namespace OnlineShop.Application.Models
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
