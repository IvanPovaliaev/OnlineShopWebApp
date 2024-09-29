using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public OrderStatus Status { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int? Entrance { get; set; }
        public int? Floor { get; set; }
        public int? Apartment { get; set; }
        public int PostCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? ReservePhone { get; set; }
        public string AdditionalInfo { get; set; }
        public List<CartPosition> Positions { get; set; }
        public long Article => GetArticle();

        public Order()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
            Status = OrderStatus.Created;
        }

        /// <summary>
        /// Get Article
        /// </summary>
        /// <returns>positive 64-bit integer</returns>
        private long GetArticle()
        {
            var bytes = Id.ToByteArray();
            var article = BitConverter.ToInt64(bytes, 0);
            return Math.Abs(article);
        }
    }
}
