using System;

namespace OnlineShop.Db.Models
{
    public class UserDeliveryInfo
    {
        public Guid Id { get; init; }
        public string City { get; set; }
        public string Address { get; set; }
        public int? Entrance { get; set; }
        public int? Floor { get; set; }
        public int? Apartment { get; set; }
        public string PostCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? ReservePhone { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
