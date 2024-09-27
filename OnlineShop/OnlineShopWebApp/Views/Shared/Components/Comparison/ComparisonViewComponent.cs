using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

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

        public IViewComponentResult Invoke()
        {
            var comparisonsCount = _comparisonsService
                .GetAll(_userId)
                .Count;
            return View("Comparison", comparisonsCount);
        }
    }
}
