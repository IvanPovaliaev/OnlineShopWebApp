using OnlineShop.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Application.Interfaces
{
	public interface IFavoritesService
	{
		/// <summary>
		/// Get all Favorites for target user by Id
		/// </summary>
		/// <returns>List of FavoriteProductViewModel for target user</returns>
		/// <param name="userId">User Id (GUID)</param>
		Task<List<FavoriteProductViewModel>> GetAllAsync(string userId);

		/// <summary>
		/// Create a new FavoriteProduct for related user.
		/// </summary>        
		/// <param name="productId">Product Id (GUID)</param>
		/// <param name="userId">User Id (GUID)</param>
		Task<Guid?> CreateAsync(Guid productId, string userId);

		/// <summary>
		/// Delete target FavoriteProduct by Id
		/// </summary>        
		/// <param name="id">FavoriteProduct Id (GUID)</param>
		Task<bool> DeleteAsync(Guid id);

		/// <summary>
		/// Delete all FavoriteProducts for related user.
		/// </summary>        
		/// <param name="userId">User Id (GUID)</param>
		Task<bool> DeleteAllAsync(string userId);
	}
}
