using OnlineShop.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Admin
{
    public class AdminEditUserViewModel : EditUserViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string RoleName { get; set; }
    }
}
