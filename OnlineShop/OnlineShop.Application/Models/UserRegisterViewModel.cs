using OnlineShop.Application.Models.Abstractions;

namespace OnlineShop.Application.Models
{
    public class UserRegisterViewModel : RegisterViewModel
    {
        public string ReturnUrl { get; set; }
    }
}
