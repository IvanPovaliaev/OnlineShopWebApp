using LinqSpecs;
using OnlineShop.Db.Models;
using System;
using System.Linq.Expressions;

namespace OnlineShopWebApp.Helpers.Specifications
{
    public class ProductsBySearchSpecification(string searchQuery) : Specification<Product>
    {
        public string SearchQuery { get; init; } = searchQuery;

        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.Name.Contains(SearchQuery);
        }
    }
}
