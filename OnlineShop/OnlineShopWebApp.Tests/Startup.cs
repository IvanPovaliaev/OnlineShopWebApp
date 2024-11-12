using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
        }
    }
}
