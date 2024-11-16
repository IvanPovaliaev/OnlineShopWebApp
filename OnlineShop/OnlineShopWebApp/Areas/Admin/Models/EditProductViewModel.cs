using Microsoft.AspNetCore.Http;
using System;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class EditProductViewModel : AdminProductViewModel
    {
        public Guid Id { get; init; }
        public IFormFile? UploadedImage { get; init; }
    }
}
