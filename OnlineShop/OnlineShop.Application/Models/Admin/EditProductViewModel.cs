using Microsoft.AspNetCore.Http;
using OnlineShop.Application.Models.Abstractions;
using System;
using System.Collections.Generic;

namespace OnlineShop.Application.Models.Admin
{
	public class EditProductViewModel : AdminProductViewModel
	{
		public Guid Id { get; init; }
		public List<ImageViewModel> Images { get; set; } = [];
		public List<IFormFile>? UploadedImages { get; init; }
	}
}
