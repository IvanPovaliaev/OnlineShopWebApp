using OnlineShopWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AdminEditUserViewModel : EditUserViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string RoleName { get; set; }
    }
}
