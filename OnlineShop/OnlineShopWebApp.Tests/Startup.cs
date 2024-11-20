using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OnlineShopWebApp.Interfaces;
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
            services.AddTransient<Mock<IAccountsService>>();
        }
    }
}
