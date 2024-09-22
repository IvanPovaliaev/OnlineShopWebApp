using System;

namespace OnlineShopWebApp.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string City { get; set; }
        public string Adress { get; set; }
        public int? Entrance { get; set; }
        public int? Floor { get; set; }
        public int? Apartament { get; set; }
        public int Index { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? ReservePhone { get; set; }
        public string AdditionalInfo { get; set; }

        public Order()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }
    }
}
