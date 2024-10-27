using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Interfaces
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
        /// <param name="id">ComparisonProduct Id (guid)</param>
        ComparisonProduct Get(Guid id);

        /// <summary>
        /// Create a new ComparisonProduct
        /// </summary>
        /// <param name="product">Target ComparisonProduct</param>
        void Create(ComparisonProduct product);

        /// <summary>
        /// Delete a ComparisonProduct by Id.
        /// </summary>
        /// <param name="id">ComparisonProduct Id (guid)</param>
        void Delete(Guid id);

        /// <summary>
        /// Delete all ComparisonProducts by userId.
        /// </summary>
        /// <param name="userId">User Id (guid)</param>
        void DeleteAll(Guid userId);
    }
}
