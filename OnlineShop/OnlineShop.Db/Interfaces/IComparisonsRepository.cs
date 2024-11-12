using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Interfaces
{
    public interface IComparisonsRepository
    {
        /// <summary>
        /// Get all ComparisonsProduct
        /// </summary>
        /// <returns>List of all products</returns>
        Task<List<ComparisonProduct>> GetAllAsync();

        /// <summary>
        /// Get a ComparisonProduct by Id
        /// </summary>
        /// <returns>Target ComparisonProduct</returns>
        /// <param name="id">ComparisonProduct Id (guid)</param>
        Task<ComparisonProduct> GetAsync(Guid id);

        /// <summary>
        /// Create a new ComparisonProduct
        /// </summary>
        /// <param name="product">Target ComparisonProduct</param>
        Task CreateAsync(ComparisonProduct product);

        /// <summary>
        /// Delete a ComparisonProduct by Id.
        /// </summary>
        /// <param name="id">ComparisonProduct Id (guid)</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Delete all ComparisonProducts by userId.
        /// </summary>
        /// <param name="userId">User Id (guid)</param>
        Task DeleteAllAsync(string userId);
    }
}
