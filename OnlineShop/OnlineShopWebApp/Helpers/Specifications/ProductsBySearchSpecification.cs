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
            if (long.TryParse(SearchQuery, out var result))
            {
                return product => product.Name.Contains(SearchQuery) || product.Article.ToString()
                                                                                       .Contains(SearchQuery);
            }
            return product => product.Name.Contains(SearchQuery);
        }
    }
}
