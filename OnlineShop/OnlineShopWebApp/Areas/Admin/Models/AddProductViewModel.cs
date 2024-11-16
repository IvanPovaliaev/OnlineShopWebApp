using Microsoft.AspNetCore.Http;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AddProductViewModel : AdminProductViewModel
    {
        public IFormFile? UploadedImage { get; init; }
    }
}
