using LinqSpecs;
using OnlineShop.Db.Models;
using System;
using System.Linq.Expressions;

namespace OnlineShop.Application.Helpers.Specifications
{
    public class FavoritesByUserIdSpecification(string userId) : Specification<FavoriteProduct>
    {
        public string UserId { get; init; } = userId;

        public override Expression<Func<FavoriteProduct, bool>> ToExpression()
        {
            return favorite => favorite.UserId == UserId;
        }
    }
}
