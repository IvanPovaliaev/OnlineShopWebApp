using LinqSpecs;
using OnlineShop.Db.Models;
using System;
using System.Linq.Expressions;

namespace OnlineShopWebApp.Helpers.Specifications
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
