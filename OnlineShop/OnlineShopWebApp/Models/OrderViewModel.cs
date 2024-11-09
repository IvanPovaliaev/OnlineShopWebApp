using OnlineShopWebApp.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Models
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public OrderStatusViewModel Status { get; set; }
        public UserDeliveryInfoViewModel Info { get; set; }
        public List<OrderPositionViewModel> Positions { get; set; }
        public long Article => GetArticle();
        public decimal TotalCost
        {
            get => Positions?.Sum(p => p.Cost) ?? 0;
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
