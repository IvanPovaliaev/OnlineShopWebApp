using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } //Для дальнейшей привязки пользователя
        public List<CartPosition> Positions { get; set; }
        public decimal TotalCost
        {
            get => Positions.Sum(p => p.Cost);
        }

        public Cart(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Positions = [];
        }
    }
}
