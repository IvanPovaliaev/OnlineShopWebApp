using Microsoft.AspNetCore.Hosting;
using OnlineShop.Application.Interfaces;

namespace OnlineShopWebApp.Helpers
{
    public class HostingEnvironmentService : IHostingEnvironmentService
    {
        private readonly IWebHostEnvironment _env;

        public HostingEnvironmentService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string GetWebRootPath() => _env.WebRootPath;
    }
}
