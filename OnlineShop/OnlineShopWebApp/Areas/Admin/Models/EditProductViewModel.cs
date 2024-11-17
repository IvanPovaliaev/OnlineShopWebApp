using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class EditProductViewModel : AdminProductViewModel
    {
        public Guid Id { get; init; }
        public List<IFormFile>? UploadedImages { get; init; }
    }
}
