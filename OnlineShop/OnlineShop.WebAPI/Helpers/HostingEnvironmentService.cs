using OnlineShop.Application.Interfaces;

namespace OnlineShop.WebAPI.Helpers
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
