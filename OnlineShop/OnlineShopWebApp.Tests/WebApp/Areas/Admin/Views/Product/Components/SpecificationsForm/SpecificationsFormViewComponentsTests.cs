using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using OnlineShop.Application.Helpers.SpecificationsRules;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShopWebApp.Areas.Admin.Views.Product.Components.SpecificationsForm;
using System.Collections.Generic;
using Xunit;

namespace OnlineShopWebApp.Tests.WebApp.Areas.Admin.Views.Product.Components.SpecificationsForm
{
    public class SpecificationsFormViewComponentTests
    {
        private readonly Mock<IProductsService> _productsServiceMock;
        private readonly SpecificationsFormViewComponent _viewComponent;

        public SpecificationsFormViewComponentTests(Mock<IProductsService> productsServiceMock)
        {
            _productsServiceMock = productsServiceMock;
            _viewComponent = new SpecificationsFormViewComponent(_productsServiceMock.Object);
        }

        [Fact]
        public void Invoke_WhenCalled_ReturnsViewWithSpecificationsAndRules()
        {
            // Arrange
            var category = ProductCategoriesViewModel.Motherboards;
            var rules = new MotherboardSpecificationsRules();
            var specifications = new Dictionary<string, string>
            {
                { "Manufacturer", "1" },
                { "ManufacturerCode", "1" },
                { "Socket", "1" },
                { "Chipset", "1" },
            };

            _productsServiceMock.Setup(s => s.GetSpecificationsRules(category))
                                .Returns(rules);

            var specificationsWithCategory = (specifications, category);

            // Act
            var result = _viewComponent.Invoke(specificationsWithCategory);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("SpecificationsForm", viewResult.ViewName);

            var model = Assert.IsType<(Dictionary<string, string>, IProductSpecificationsRules)>(viewResult.ViewData.Model);
            Assert.Equal(specifications, model.Item1);
            Assert.Equal(rules, model.Item2);
        }
    }
}
