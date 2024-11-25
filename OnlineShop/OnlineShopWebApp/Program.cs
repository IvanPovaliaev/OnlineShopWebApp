using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShop.Db;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Db.Repositories;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Redis;
using OnlineShopWebApp.Services;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", "Online Shop"));

builder.Services.AddIdentity<User, OnlineShop.Db.Models.Role>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

var redisConfiguration = ConfigurationOptions.Parse(builder.Configuration.GetSection("Redis:ConnectionString").Value);
redisConfiguration.AbortOnConnectFail = false;
redisConfiguration.ConnectTimeout = 10;

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConfiguration);
});

builder.Services.AddSingleton<RedisCacheService>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.LoginPath = "/Account/Unauthorized";
    options.LogoutPath = "/Account/Unauthorized";
    options.Cookie = new CookieBuilder
    {
        IsEssential = true
    };
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped<IProductsRepository, ProductsDbRepository>();
builder.Services.AddTransient<IProductsService, ProductsService>();

builder.Services.AddScoped<ICartsRepository, CartsDbRepository>();
builder.Services.AddTransient<ICartsService, CartsService>();
builder.Services.AddTransient<ICookieCartsService, CookieCartsService>();

builder.Services.AddScoped<IOrdersRepository, OrdersDbRepository>();
builder.Services.AddTransient<IOrdersService, OrdersService>();

builder.Services.AddScoped<IComparisonsRepository, ComparisonsDbRepository>();
builder.Services.AddTransient<IComparisonsService, ComparisonsService>();

builder.Services.AddScoped<IFavoritesRepository, FavoritesDbRepository>();
builder.Services.AddTransient<IFavoritesService, FavoritesService>();

builder.Services.AddTransient<IRolesService, RolesService>();

builder.Services.AddTransient<IAccountsService, AccountsService>();

builder.Services.AddTransient<HashService>();
builder.Services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();

builder.Services.AddTransient<IExcelService, ClosedXMLExcelService>();
builder.Services.AddTransient<AuthenticationHelper>();

builder.Services.AddTransient<ImagesProvider>();

var mailSetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSetting);

builder.Services.AddTransient<IMailService, EmailService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<OnlineShop.Db.Models.Role>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    var identityInitializer = new IdentityInitializer(configuration);
    await identityInitializer.InitializeAsync(userManager, roleManager);
}

using (var scope = app.Services.CreateScope())
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ApplicationName", "Online Shop")
        .WriteToDatabase(databaseType.ToLower(), connection!)
        .CreateLogger();
}

app.Run();