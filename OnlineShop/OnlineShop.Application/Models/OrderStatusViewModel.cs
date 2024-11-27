using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models
{
    public enum OrderStatusViewModel
    {
        [Display(Name = "Создан")]
        Created,

        [Display(Name = "Подтверждён")]
        Confirmed,

        [Display(Name = "В пути")]
        Delivering,

        [Display(Name = "Доставлен")]
        Delivered,

        [Display(Name = "Отменён")]
        Canceled
    }
}
