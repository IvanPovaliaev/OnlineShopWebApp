using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Application.Helpers;
using OnlineShop.Application.Helpers.Handlers;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Options;
using OnlineShop.Application.Services;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using OnlineShop.Infrastructure.Data.Repositories;
using OnlineShop.Infrastructure.Email;
using OnlineShop.Infrastructure.Excel;
using OnlineShop.Infrastructure.Redis;
using OnlineShop.Infrastructure.ReviewApiService;
using StackExchange.Redis;
using System;
using System.Globalization;
using System.Net.Http.Headers;

namespace OnlineShop.Infrastructure.CommonDI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = configuration.GetSection("Redis");
            services.Configure<RedisSettings>(redisSettings);

            services.AddSingleton<RedisService>();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UsersOnRoleDeletedHandler).Assembly));

            services.AddScoped<IProductsRepository, ProductsDbRepository>();
            services.AddTransient<IProductsService, ProductsService>();
            //services.Decorate<IProductsService, RedisProductsService>();

            services.Scan(scan => scan
                    .FromAssemblyOf<IProductSpecificationsRules>()
                    .AddClasses(classes => classes.AssignableTo<IProductSpecificationsRules>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            services.AddScoped<ICartsRepository, CartsDbRepository>();
            services.AddTransient<ICartsService, CartsService>();

            services.AddScoped<IOrdersRepository, OrdersDbRepository>();
            services.AddTransient<IOrdersService, OrdersService>();

            services.AddScoped<IComparisonsRepository, ComparisonsDbRepository>();
            services.AddTransient<IComparisonsService, ComparisonsService>();

            services.AddScoped<IFavoritesRepository, FavoritesDbRepository>();
            services.AddTransient<IFavoritesService, FavoritesService>();

            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<IAccountsService, AccountsService>();

            services.AddTransient<HashService>();
            services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();

            services.AddTransient<ImagesProvider>();
            services.AddTransient<IExcelService, ClosedXMLExcelService>();

            var mailSetting = configuration.GetSection(nameof(MailSettings));
            services.Configure<MailSettings>(mailSetting);
            services.AddTransient<IMailService, EmailService>();

            services.Configure<ImagesStorage>(
                configuration.GetSection(nameof(ImagesStorage)));

            var redisConfiguration = ConfigurationOptions.Parse(configuration.GetSection("Redis:ConnectionString").Value);

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConfiguration);
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US")
                };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });


            var reviewsService = configuration.GetSection("Microservices:ReviewsService");
            services.Configure<ReviewsSettings>(reviewsService);

            services.AddHttpClient("ReviewsService", client =>
            {
                var reviewSettings = reviewsService.Get<ReviewsSettings>();
                client.BaseAddress = new Uri(reviewSettings!.Url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddScoped<IReviewsService, ReviewsService>();
            services.AddSingleton<ReviewTokenStorage>();

            return services;
        }
    }
}
