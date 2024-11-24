using LinqSpecs;
using OnlineShop.Db.Models;
using System;
using System.Linq.Expressions;

namespace OnlineShopWebApp.Helpers.Specifications
{
    public class OrdersByUserIdSpecification(string userId) : Specification<Order>
    {
        public string UserId { get; init; } = userId;

        public override Expression<Func<Order, bool>> ToExpression()
        {
            return order => order.UserId == UserId;
        }
    }
}
