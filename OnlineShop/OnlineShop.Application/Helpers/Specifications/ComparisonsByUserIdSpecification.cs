using LinqSpecs;
using OnlineShop.Domain.Models;
using System;
using System.Linq.Expressions;

namespace OnlineShop.Application.Helpers.Specifications
{
    public class ComparisonsByUserIdSpecification(string userId) : Specification<ComparisonProduct>
    {
        public string UserId { get; init; } = userId;

        public override Expression<Func<ComparisonProduct, bool>> ToExpression()
        {
            return comparison => comparison.UserId == UserId;
        }
    }
}
