using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IComparisonRepository
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
        void Create(ComparisonProduct product);

        /// <summary>
        /// Delete a ComparisonProduct by Id.
        /// </summary>  
        void Delete(Guid comparisonId);
    }
}
