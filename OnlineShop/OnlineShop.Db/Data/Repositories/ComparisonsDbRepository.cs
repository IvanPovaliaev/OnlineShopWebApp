using LinqSpecs;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Data.Repositories
{
	public class ComparisonsDbRepository : IComparisonsRepository
	{
		private readonly DatabaseContext _databaseContext;

		public ComparisonsDbRepository(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		public async Task<List<ComparisonProduct>> GetAllAsync(Specification<ComparisonProduct>? specification = null)
		{
			var query = _databaseContext.ComparisonProducts.AsQueryable();

			if (specification is not null)
			{
				query = query.Where(specification.ToExpression());
			}

			return await query.Include(f => f.Product)
							  .ThenInclude(p => p.Images)
							  .ToListAsync();
		}

		public async Task<ComparisonProduct> GetAsync(Guid id)
		{
			return await _databaseContext.ComparisonProducts.Include(c => c.Product)
															.ThenInclude(p => p.Images)
															.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<Guid?> CreateAsync(ComparisonProduct product)
		{
			await _databaseContext.ComparisonProducts.AddAsync(product);
			var result = await _databaseContext.SaveChangesAsync();
			return result > 0 ? product.Id : null;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var comparison = await _databaseContext.ComparisonProducts.FindAsync(id);
			if (comparison is null)
			{
				return false;
			}

			_databaseContext.ComparisonProducts.Remove(comparison!);
			await _databaseContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAllAsync(string userId)
		{
			var comparisons = await _databaseContext.ComparisonProducts.Where(c => c.UserId == userId)
																	   .ToArrayAsync();
			_databaseContext.ComparisonProducts.RemoveRange(comparisons);
			return await _databaseContext.SaveChangesAsync() > 0;
		}
	}
}
