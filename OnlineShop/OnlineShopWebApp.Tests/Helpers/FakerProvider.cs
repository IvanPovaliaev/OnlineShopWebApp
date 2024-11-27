using Bogus;
using OnlineShop.Db;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Tests.Helpers
{
    public class FakerProvider
    {
        public string UserId { get; }
        public Faker<Product> ProductFaker { get; }
        public Faker<CartPosition> CartPositionFaker { get; }
        public Faker<ComparisonProduct> ComparisonProductFaker { get; }
        public Faker<FavoriteProduct> FavoriteProductFaker { get; }
        public Faker<Order> OrderFaker { get; }
        public Faker<UserDeliveryInfo> UserDeliveryInfoFaker { get; }
        public Faker<Role> RoleFaker { get; }
        public Faker<User> UserFaker { get; }

        public const int PositionsCount = 10;
        public const int ProductsCount = 10;
        public const int OrdersCount = 10;
        public const int RolesCount = 10;
        public const int UsersCount = 10;
        public Cart FakeCart { get; }
        public List<Product> FakeProducts { get; }
        public List<ComparisonProduct> FakeComparisonProducts { get; }
        public List<FavoriteProduct> FakeFavoriteProducts { get; }
        public List<Order> FakeOrders { get; }
        public List<Role> FakeRoles { get; }
        public List<User> FakeUsers { get; }

        public FakerProvider()
        {
            UserId = Guid.NewGuid().ToString();

            ProductFaker = new Faker<Product>()
                                .RuleFor(p => p.Id, f => Guid.NewGuid())
                                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                                .RuleFor(p => p.Cost, f => f.Finance.Amount())
                                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                                .RuleFor(p => p.Category, f => f.PickRandom<ProductCategories>());

            CartPositionFaker = new Faker<CartPosition>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Product, ProductFaker.Generate())
                .RuleFor(p => p.Quantity, f => f.Random.Int(1, 10));

            RoleFaker = new Faker<Role>().RuleFor(r => r.Id, f => f.Random.Guid().ToString())
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
                .RuleFor(o => o.UserId, f => f.Random.Guid().ToString())
                .RuleFor(o => o.CreationDate, f => f.Date.Past())
                .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>())
                .RuleFor(o => o.Info, f => UserDeliveryInfoFaker.Generate());

            var fakeCartPositions = CartPositionFaker.Generate(PositionsCount);

            FakeCart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                Positions = fakeCartPositions
            };

            FakeProducts = ProductFaker.Generate(ProductsCount);
            FakeComparisonProducts = ComparisonProductFaker.Generate(ProductsCount);
            FakeFavoriteProducts = FavoriteProductFaker.Generate(ProductsCount);
            FakeOrders = OrderFaker.Generate(OrdersCount);
            FakeRoles = RoleFaker.Generate(RolesCount);
            var userRole = new Role()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Constants.UserRoleName,
                CanBeDeleted = false
            };
            FakeRoles.Add(userRole);

            UserFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid().ToString())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.UserName, (f, u) => u.Email)
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.FullName, f => f.Name.FullName())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.AvatarUrl, f => f.Image.PicsumUrl());

            FakeUsers = UserFaker.Generate(UsersCount);
        }
    }
}
