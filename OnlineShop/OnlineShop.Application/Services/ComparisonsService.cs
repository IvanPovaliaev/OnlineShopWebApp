using AutoMapper;
using OnlineShop.Application.Helpers.Specifications;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
	public class ComparisonsService : IComparisonsService
	{
		private readonly IComparisonsRepository _comparisonsRepository;
		private readonly IMapper _mapper;
		private readonly IProductsService _productsService;

		public ComparisonsService(IComparisonsRepository comparisonsRepository, IMapper mapper, IProductsService productsService)
		{
			_comparisonsRepository = comparisonsRepository;
			_mapper = mapper;
			_productsService = productsService;
		}

		public async Task<List<ComparisonProductViewModel>> GetAllAsync(string userId)
		{
			var specification = new ComparisonsByUserIdSpecification(userId);

			var comparisons = await _comparisonsRepository.GetAllAsync(specification);
			return comparisons.Select(_mapper.Map<ComparisonProductViewModel>)
							  .ToList();
		}

		public async Task<ILookup<ProductCategoriesViewModel, ComparisonProductViewModel>> GetGroupsAsync(string userId)
		{
			return (await GetAllAsync(userId)).ToLookup(c => c.Product.Category);
		}

		public async Task CreateAsync(Guid productId, string userId)
		{
			var product = await _productsService.GetAsync(productId);
			if (await IsProductExistsAsync(product, userId))
			{
				return;
			}

			var comparison = new ComparisonProduct()
			{
				UserId = userId,
				Product = product
			};

			await _comparisonsRepository.CreateAsync(comparison);
		}

		public async Task DeleteAsync(Guid comparisonId)
		{
			await _comparisonsRepository.DeleteAsync(comparisonId);
		}

		public async Task DeleteAllAsync(string userId)
		{
			await _comparisonsRepository.DeleteAllAsync(userId);
		}

		/// <summary>
		/// Checks if the given product exists in users comparison products
		/// </summary>
		/// <returns>true if product exists; otherwise returns false</returns>
		/// <param name="product">Target Product</param>
		/// <param name="userId">User Id (GUID)</param>
		private async Task<bool> IsProductExistsAsync(Product product, string userId)
		{
			return (await GetAllAsync(userId)).Any(c => c.Product.Id == product.Id);
		}
	}
}