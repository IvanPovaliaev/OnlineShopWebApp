using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class ComparisonViewComponent : ViewComponent
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly ComparisonsService _comparisonsService;

        public ComparisonViewComponent(ComparisonsService comparisonsService)
        {
            _comparisonsService = comparisonsService;
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
