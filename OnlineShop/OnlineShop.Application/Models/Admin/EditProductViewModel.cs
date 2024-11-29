using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace OnlineShop.Application.Models.Admin
{
    public class EditProductViewModel : AdminProductViewModel
    {
        public Guid Id { get; init; }
        public List<IFormFile>? UploadedImages { get; init; }
    }
}
