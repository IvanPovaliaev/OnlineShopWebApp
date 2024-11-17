using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class AddProductViewModel : AdminProductViewModel
    {
        public List<string> ImageUrls { get; set; } = [];
        public List<IFormFile>? UploadedImages { get; init; }
    }
}
