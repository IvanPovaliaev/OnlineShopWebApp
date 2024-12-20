﻿using LinqSpecs;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Domain.Interfaces
{
    public interface IComparisonsRepository
    {
        /// <summary>
        /// Get all ComparisonsProduct
        /// </summary>
        /// <returns>List of all products</returns>
        /// <param name="specification">Specification for comparisons</param>
        Task<List<ComparisonProduct>> GetAllAsync(Specification<ComparisonProduct>? specification = null);

        /// <summary>
        /// Get a ComparisonProduct by Id
        /// </summary>
        /// <returns>Target ComparisonProduct</returns>
        /// <param name="id">ComparisonProduct Id (guid)</param>
        Task<ComparisonProduct?> GetAsync(Guid id);

        /// <summary>
        /// Create a new ComparisonProduct
        /// </summary>
        /// <param name="product">Target ComparisonProduct</param>
        Task<Guid?> CreateAsync(ComparisonProduct product);

        /// <summary>
        /// Delete a ComparisonProduct by Id.
        /// </summary>
        /// <param name="id">ComparisonProduct Id (guid)</param>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Delete all ComparisonProducts by userId.
        /// </summary>
        /// <param name="userId">User Id (guid)</param>
        Task<bool> DeleteAllAsync(string userId);
    }
}
