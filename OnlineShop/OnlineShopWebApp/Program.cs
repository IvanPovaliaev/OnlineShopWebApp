using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Options;
using OnlineShop.Application.Services;
using OnlineShop.Domain.Models;
using OnlineShop.Infrastructure.CommonDI;
using OnlineShop.Infrastructure.Data;
using OnlineShopWebApp.Helpers;
using Serilog;
using System;
using System.Collections.Generic;

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

builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddCommonServices(builder.Configuration);

builder.Services.AddOptions<ConfigCatOptions>()
                .Bind(builder.Configuration.GetSection(nameof(ConfigCatOptions)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

//builder.Services.AddSingleton<IFeatureDefinitionProvider, ConfigCatFeatureDefinitionProvider>();
builder.Services.AddFeatureManagement();

builder.Services.AddOptions<CookieCartOptions>()
                .Bind(builder.Configuration.GetSection("CookiesSettings"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddTransient<ICookieCartsService, CookieCartsService>();
builder.Services.AddTransient<AuthenticationHelper>();

builder.Services.AddScoped<IHostingEnvironmentService, HostingEnvironmentService>();

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
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
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