using LinqSpecs;
using OnlineShop.Domain.Models;
using System;
using System.Linq.Expressions;

namespace OnlineShop.Application.Helpers.Specifications
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
