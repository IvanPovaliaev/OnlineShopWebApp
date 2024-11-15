using System;

namespace OnlineShopWebApp.Models
{
    public class CookieCartPositionViewModel
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; set; }
    }
}
