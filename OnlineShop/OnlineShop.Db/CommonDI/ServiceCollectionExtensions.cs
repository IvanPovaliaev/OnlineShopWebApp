using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Application.Helpers;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Services;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using OnlineShop.Infrastructure.Data.Repositories;
using OnlineShop.Infrastructure.Email;
using System.Globalization;
using System.Reflection;

namespace OnlineShop.Infrastructure.CommonDI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped<IProductsRepository, ProductsDbRepository>();
            services.AddTransient<IProductsService, ProductsService>();

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

            var mailSetting = configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSetting);
            services.AddTransient<IMailService, EmailService>();

            services.Scan(scan => scan
                .FromAssemblyOf<IProductSpecificationsRules>()
                .AddClasses(classes => classes.AssignableTo<IProductSpecificationsRules>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());

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

            return services;
        }
    }
}
