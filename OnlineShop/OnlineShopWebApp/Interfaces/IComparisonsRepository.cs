using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IComparisonsRepository
    {
        /// <summary>
        /// Get all ComparisonsProduct
        /// </summary>
        /// <returns>List of all products</returns>
        List<ComparisonProduct> GetAll();

        /// <summary>
        /// Get a ComparisonProduct by Id
        /// </summary>
        /// <returns>Target ComparisonProduct</returns>
        /// <param name="comparisonId">ComparisonProduct Id (guid)</param>
        ComparisonProduct Get(Guid comparisonId);

        /// <summary>
        /// Create a new ComparisonProduct
        /// </summary>
        /// <param name="product">Target ComparisonProduct</param>
        void Create(ComparisonProduct product);

        /// <summary>
        /// Delete a ComparisonProduct by Id.
        /// </summary>
        /// <param name="comparisonId">ComparisonProduct Id (guid)</param>
        void Delete(Guid comparisonId);

        /// <summary>
        /// Delete all ComparisonProducts by userId.
        /// </summary>
        /// <param name="userId">User Id (guid)</param>
        void DeleteAll(Guid userId);
    }
}
