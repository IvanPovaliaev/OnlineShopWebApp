using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class FeatureController : Controller
    {
        private readonly IFeatureManager _featureManager;
        private readonly IConfiguration _configuration;

        public FeatureController(IFeatureManager featureManager, IConfiguration configuration)
        {
            _featureManager = featureManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Open Admin Settings page
        /// </summary>
        /// <returns>Admin Settings View</returns>
        public async Task<IActionResult> Index()
        {
            var featureNames = _featureManager.GetFeatureNamesAsync();

            var features = new List<FeatureFlagModel>();

            await foreach (var featureName in featureNames)
            {
                var isEnabled = await _featureManager.IsEnabledAsync(featureName);
                features.Add(new FeatureFlagModel
                {
                    Name = featureName,
                    IsEnabled = isEnabled
                });
            }

            return View(features);
        }

        /// <summary>
        /// Update 
        /// </summary>
        /// <returns>Admins products View</returns>
        [HttpPost]
        public async Task<IActionResult> Update(List<FeatureFlagModel> features)
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var jsonObject = JObject.Parse(json);

            var featureManagement = jsonObject["FeatureManagement"] as JObject;
            var featureNames = _featureManager.GetFeatureNamesAsync();

            //await foreach (var featureName in featureNames)
            //{
            //    var isEnabled = await _featureManager.IsEnabledAsync(featureName);
            //    features.Add(new FeatureFlagModel
            //    {
            //        Name = featureName,
            //        IsEnabled = isEnabled
            //    });
            //}

            if (featureManagement != null)
            {
                foreach (var feature in features)
                {
                    featureManagement[feature.Name] = feature.IsEnabled;
                }
            }

            await using (var writer = new StreamWriter(jsonPath, false))
            {
                await writer.WriteAsync(jsonObject.ToString(Formatting.Indented));
            }

            var root = _configuration as IConfigurationRoot;
            root?.Reload();

            return RedirectToAction("Index");
        }
    }
}
