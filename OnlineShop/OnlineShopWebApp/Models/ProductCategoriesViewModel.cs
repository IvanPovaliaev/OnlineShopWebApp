using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public enum ProductCategoriesViewModel
    {
        [Display(Name = "Видеокарты")]
        GraphicCards,

        [Display(Name = "Процессоры")]
        Processors,

        [Display(Name = "Материнские платы")]
        Motherboards,

        [Display(Name = "SDD")]
        SSD,

        [Display(Name = "HDD")]
        HDD,

        [Display(Name = "Оперативная память")]
        RAM,

        [Display(Name = "Блоки питания")]
        PowerSupplies
    }
}
