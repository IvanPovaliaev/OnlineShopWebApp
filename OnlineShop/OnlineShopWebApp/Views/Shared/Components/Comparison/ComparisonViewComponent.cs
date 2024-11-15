using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class ComparisonViewComponent : ViewComponent
    {
        private readonly string _userId;
        private readonly ComparisonsService _comparisonsService;

        public ComparisonViewComponent(ComparisonsService comparisonsService, IHttpContextAccessor httpContextAccessor)
        {
            _comparisonsService = comparisonsService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Show comparison icon component on View;
        /// </summary>
        /// <returns>CartViewComponent</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var comparisonsCount = (await _comparisonsService.GetAllAsync(_userId))
                                                             .Count;
            return View("Comparison", comparisonsCount);
        }
    }
}
