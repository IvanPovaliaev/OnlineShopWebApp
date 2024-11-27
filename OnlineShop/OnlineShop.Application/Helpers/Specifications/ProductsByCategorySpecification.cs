using LinqSpecs;
using OnlineShop.Domain.Models;
using System;
using System.Linq.Expressions;

namespace OnlineShop.Application.Helpers.Specifications
{
    public class ProductByCategorySpecification(ProductCategories category) : Specification<Product>
    {
        public ProductCategories Category { get; init; } = category;

        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.Category == Category;
        }
    }
}
