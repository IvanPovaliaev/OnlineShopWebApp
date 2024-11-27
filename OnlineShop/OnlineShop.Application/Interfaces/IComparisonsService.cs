using OnlineShop.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Interfaces
{
    public interface IComparisonsService
    {
        /// <summary>
        /// Get all Comparisons for target user by Id
        /// </summary>
        /// <returns>List of ComparisonProductViewModel for target user</returns>
        /// <param name="userId">User Id (GUID)</param>
        Task<List<ComparisonProductViewModel>> GetAllAsync(string userId);

        /// <summary>
        /// Get all Comparisons groups for target user by Id
        /// </summary>
        /// <returns>ILookup object of ComparisonProducts grouping by ProductCategory </returns>
        /// <param name="userId">User Id (GUID)</param>
        Task<ILookup<ProductCategoriesViewModel, ComparisonProductViewModel>> GetGroupsAsync(string userId);

        /// <summary>
        /// Create a new ComparisonProduct for related user.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        Task CreateAsync(Guid productId, string userId);

        /// <summary>
        /// Delete target ComparisonProduct by Id
        /// </summary>        
        /// <param name="comparisonId">ComparisonProduct Id (GUID)</param>
        Task DeleteAsync(Guid comparisonId);

        /// <summary>
        /// Delete all ComparisonProducts for related user.
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        Task DeleteAllAsync(string userId);
    }
}
