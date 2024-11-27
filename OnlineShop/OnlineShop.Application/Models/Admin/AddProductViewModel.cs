using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace OnlineShop.Application.Models.Admin
{
    public class AddProductViewModel : AdminProductViewModel
    {
        public List<IFormFile>? UploadedImages { get; init; }
    }
}
