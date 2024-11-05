using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShop.Db;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Repositories;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", "Online Shop"));

var databaseProvider = builder.Configuration["DatabaseProvider"];
var connection = builder.Configuration.GetConnectionString(databaseProvider!);
var databaseTypes = builder.Configuration.GetSection("DatabaseTypes")
                                         .Get<Dictionary<string, string>>();

if (!databaseTypes.TryGetValue(databaseProvider!, out var databaseType))
{
    throw new InvalidOperationException("Invalid database provider");
}

switch (databaseType.ToLower())
{
    case "postgresql":
        builder.Services.AddDbContext<DatabaseContext, PostgreSQLContext>(options => options.UseNpgsql(connection), ServiceLifetime.Scoped);
        break;
    case "mssql":
        builder.Services.AddDbContext<DatabaseContext, MsSQLContext>(options => options.UseSqlServer(connection), ServiceLifetime.Scoped);
        break;
    case "mysql":
        builder.Services.AddDbContext<DatabaseContext, MySQLContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)), ServiceLifetime.Scoped);
        break;
    default:
        throw new InvalidOperationException("Invalid database type");
}

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllersWithViews();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped<IProductsRepository, ProductsDbRepository>();
builder.Services.AddTransient<ProductsService>();

builder.Services.AddScoped<ICartsRepository, CartsDbRepository>();
builder.Services.AddTransient<CartsService>();

builder.Services.AddScoped<IOrdersRepository, OrdersDbRepository>();
builder.Services.AddTransient<OrdersService>();

builder.Services.AddScoped<IComparisonsRepository, ComparisonsDbRepository>();
builder.Services.AddTransient<ComparisonsService>();

builder.Services.AddScoped<IFavoritesRepository, FavoritesDbRepository>();
builder.Services.AddTransient<FavoritesService>();

builder.Services.AddScoped<IRolesRepository, RolesDbRepository>();
builder.Services.AddTransient<RolesService>();

builder.Services.AddScoped<IUsersRepository, UsersDbRepository>();
builder.Services.AddTransient<AccountsService>();

builder.Services.AddTransient<HashService>();

builder.Services.AddTransient<IExcelService, ClosedXMLExcelService>();

builder.Services.Scan(scan => scan
                .FromAssemblyOf<IProductSpecificationsRules>()
                .AddClasses(classes => classes.AssignableTo<IProductSpecificationsRules>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US")
    };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();