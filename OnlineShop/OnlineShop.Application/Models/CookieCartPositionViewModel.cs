using System;

namespace OnlineShop.Application.Models
{
    public class CookieCartPositionViewModel
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; set; }
    }
}
