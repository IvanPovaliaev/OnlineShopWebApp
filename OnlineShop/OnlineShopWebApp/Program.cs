using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShopWebApp.Helpers.SpecificationsRules;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Repositories;
using OnlineShopWebApp.Services;
using Serilog;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", "Online Shop"));

builder.Services.AddControllersWithViews();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddTransient<FileService>();
builder.Services.AddTransient<JsonRepositoryService>();

builder.Services.AddSingleton<IProductsRepository, InFileProductsRepository>();
builder.Services.AddTransient<ProductsService>();

builder.Services.AddSingleton<ICartsRepository, InFileCartsRepository>();
builder.Services.AddTransient<CartsService>();

builder.Services.AddSingleton<IOrdersRepository, InFileOrdersRepository>();
builder.Services.AddTransient<OrdersService>();

builder.Services.AddSingleton<IComparisonsRepository, InFileComparisonsRepository>();
builder.Services.AddTransient<ComparisonsService>();

builder.Services.AddSingleton<IFavoritesRepository, InFileFavoritesRepository>();
builder.Services.AddTransient<FavoritesService>();

builder.Services.AddSingleton<IRolesRepository, InFileRolesRepository>();
builder.Services.AddTransient<RolesService>();

builder.Services.AddSingleton<IUsersRepository, InFileUsersRepository>();
builder.Services.AddTransient<AccountsService>();

builder.Services.AddTransient<HashService>();

builder.Services.AddTransient<IExcelService, ClosedXMLExcelService>();

builder.Services.AddTransient<IProductSpecificationsRules, GraphicCardSpecificationsRules>();
builder.Services.AddTransient<IProductSpecificationsRules, HDDSpecificationsRules>();
builder.Services.AddTransient<IProductSpecificationsRules, MotherboardSpecificationsRules>();
builder.Services.AddTransient<IProductSpecificationsRules, PowerSupplySpecificationsRules>();
builder.Services.AddTransient<IProductSpecificationsRules, ProcessorSpecificationsRules>();
builder.Services.AddTransient<IProductSpecificationsRules, RAMSpecificationsRules>();
builder.Services.AddTransient<IProductSpecificationsRules, SSDSpecificationsRules>();

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