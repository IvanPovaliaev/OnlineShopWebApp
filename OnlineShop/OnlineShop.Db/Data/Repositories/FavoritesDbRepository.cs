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
	public class FavoritesDbRepository : IFavoritesRepository
	{
		private readonly DatabaseContext _databaseContext;

		public FavoritesDbRepository(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		public async Task<List<FavoriteProduct>> GetAllAsync(Specification<FavoriteProduct>? specification = null)
		{
			var query = _databaseContext.FavoriteProducts.AsQueryable();

			if (specification is not null)
			{
				query = query.Where(specification.ToExpression());
			}

			return await query.Include(f => f.Product)
							  .ThenInclude(p => p.Images)
							  .ToListAsync();
		}

		public async Task<FavoriteProduct> GetAsync(Guid id)
		{
			return await _databaseContext.FavoriteProducts.Include(f => f.Product)
														  .ThenInclude(p => p.Images)
														  .FirstOrDefaultAsync(f => f.Id == id);
		}

		public async Task<Guid?> CreateAsync(FavoriteProduct product)
		{
			await _databaseContext.FavoriteProducts.AddAsync(product);
			var result = await _databaseContext.SaveChangesAsync();
			return result > 0 ? product.Id : null;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var favoriteProduct = await _databaseContext.FavoriteProducts.FindAsync(id);
			if (favoriteProduct is null)
			{
				return false;
			}

			_databaseContext.FavoriteProducts.Remove(favoriteProduct);
			await _databaseContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAllAsync(string userId)
		{
			var favorites = await _databaseContext.FavoriteProducts.Where(f => f.UserId == userId)
																   .ToArrayAsync();
			_databaseContext.FavoriteProducts.RemoveRange(favorites);
			return await _databaseContext.SaveChangesAsync() > 0;
		}
	}
}
