using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Models
{
    public class Cart
    {
        public Guid Id { get; init; }
        public Guid UserId { get; set; } //Для дальнейшей привязки пользователя
        public List<CartPosition> Positions { get; set; }

        public Cart(Guid userId)
        {
            UserId = userId;
            Positions = [];
        }
    }
}
