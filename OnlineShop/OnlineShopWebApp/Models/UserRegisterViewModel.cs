using OnlineShopWebApp.Models.Abstractions;

namespace OnlineShopWebApp.Models
{
    public class UserRegisterViewModel : RegisterViewModel
    {
        public string ReturnUrl { get; set; }
    }
}
