using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Options;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Infrastructure.ReviewApiService;
using OnlineShopWebApp.Tests.Helpers;
using OnlineShopWebApp.Tests.TestModels;

namespace OnlineShopWebApp.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(TestMappingProfile));

            services.AddSingleton<FakerProvider>();
            services.AddTransient<Mock<IHttpContextAccessor>, HttpContextAccessorMock>();
            services.AddTransient<Mock<IUrlHelper>>();
            services.AddTransient<Mock<IExcelService>>();
            services.AddTransient<Mock<IMediator>>();
            services.AddTransient<Mock<IOptions<ImagesStorage>>>();
            services.AddTransient<Mock<IFeatureManager>>();

            services.AddTransient<Mock<IAccountsService>>();

            services.AddTransient<Mock<IProductsRepository>>();
            services.AddTransient<Mock<IProductsService>>();

            services.AddTransient<Mock<IOrdersRepository>>();
            services.AddTransient<Mock<IOrdersService>>();

            services.AddTransient<Mock<IComparisonsRepository>>();
            services.AddTransient<Mock<IComparisonsService>>();

            services.AddTransient<Mock<IFavoritesRepository>>();
            services.AddTransient<Mock<IFavoritesService>>();

            services.AddTransient<Mock<ICartsRepository>>();
            services.AddTransient<Mock<ICartsService>>();
            services.AddTransient<Mock<ICookieCartsService>>();

            services.AddTransient<Mock<IRolesService>>();
            services.AddTransient<Mock<IMailService>>();

            services.AddTransient<Mock<IReviewsService>>();
        }
    }
}
