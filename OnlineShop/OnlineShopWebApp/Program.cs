using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Repositories;
using OnlineShopWebApp.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<FileService>();
builder.Services.AddTransient<JsonRepositoryService>();

builder.Services.AddSingleton<ProductsEventService>();
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

builder.Services.AddTransient<AccountService>();

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
