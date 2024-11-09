using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Models
{
    public class Order
    {
        public Guid Id { get; init; }
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public OrderStatus Status { get; set; }
        public UserDeliveryInfo Info { get; set; }
        public List<OrderPosition> Positions { get; set; }

        public Order()
        {
            CreationDate = DateTime.Now.ToUniversalTime();
            Status = OrderStatus.Created;
        }
    }
}
