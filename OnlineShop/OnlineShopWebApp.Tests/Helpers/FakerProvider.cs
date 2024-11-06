using Bogus;
using OnlineShop.Db.Models;
using System;

namespace OnlineShopWebApp.Tests.Helpers
{
    public static class FakerProvider
    {
        public static Guid UserId { get; }
        public static Faker<Product> ProductFaker { get; }
        public static Faker<CartPosition> CartPositionFaker { get; }
        public static Faker<ComparisonProduct> ComparisonProductFaker { get; }
        public static Faker<FavoriteProduct> FavoriteProductFaker { get; }
        public static Faker<Order> OrderFaker { get; }
        public static Faker<UserDeliveryInfo> UserDeliveryInfoFaker { get; }
        public static Faker<Role> RoleFaker { get; }

        static FakerProvider()
        {
            UserId = Guid.NewGuid();

            ProductFaker = new Faker<Product>()
                                .RuleFor(p => p.Id, f => Guid.NewGuid())
                                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                                .RuleFor(p => p.Cost, f => f.Finance.Amount())
                                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                                .RuleFor(p => p.Category, f => f.PickRandom<ProductCategories>())
                                .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl());

            CartPositionFaker = new Faker<CartPosition>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Product, ProductFaker.Generate())
                .RuleFor(p => p.Quantity, f => f.Random.Int(1, 10));

            RoleFaker = new Faker<Role>().RuleFor(r => r.Id, f => f.Random.Guid())
                                         .RuleFor(r => r.Name, f => f.Name.JobTitle())
                                         .RuleFor(r => r.CanBeDeleted, f => f.Random.Bool());

            ComparisonProductFaker = new Faker<ComparisonProduct>()
                    .RuleFor(fp => fp.Id, f => Guid.NewGuid())
                    .RuleFor(fp => fp.UserId, f => UserId)
                    .RuleFor(fp => fp.Product, f => ProductFaker.Generate());

            FavoriteProductFaker = new Faker<FavoriteProduct>()
                .RuleFor(fp => fp.Id, f => Guid.NewGuid())
                .RuleFor(fp => fp.UserId, f => UserId)
                .RuleFor(fp => fp.Product, f => ProductFaker.Generate());

            UserDeliveryInfoFaker = new Faker<UserDeliveryInfo>()
                .RuleFor(i => i.Id, f => f.Random.Guid())
                .RuleFor(i => i.City, f => f.Address.City())
                .RuleFor(i => i.Address, f => f.Address.FullAddress())
                .RuleFor(i => i.PostCode, f => f.Address.ZipCode())
                .RuleFor(i => i.FullName, f => f.Name.FullName())
                .RuleFor(i => i.Email, f => f.Internet.Email())
                .RuleFor(i => i.Phone, f => f.Phone.PhoneNumber());

            OrderFaker = new Faker<Order>()
                .RuleFor(o => o.Id, f => f.Random.Guid())
                .RuleFor(o => o.UserId, f => f.Random.Guid())
                .RuleFor(o => o.CreationDate, f => f.Date.Past())
                .RuleFor(o => o.Info, f => UserDeliveryInfoFaker.Generate());
        }
    }
}
