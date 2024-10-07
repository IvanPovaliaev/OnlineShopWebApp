using OnlineShopWebApp.Interfaces;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class ProcessorSpecificationsRules : IProductSpecificationsRules
    {
        public List<ProductSpecificationRule> GetAll()
        {
            var manufacturerPattern = @"^(AMD|Intel)$";
            var manufacturerErrorMessage = @"Неверный производитель. Доступные варианты: AMD; Intel";

            var сoresCountPattern = @"^\d+$";
            var сoresCountErrorMessage = @"Неверное количество ядер. Пример: 1; 2; 3; 4; 8";

            var threadsCountPattern = @"^\d+$";
            var threadsCountErrorMessage = @"Неверное количество потоков. Пример: 1; 2; 3; 4; 8";

            var clockSpeedPattern = @"^\d+\sМГц$";
            var clockSpeedErrorMessage = @"Неверная тактовая частота. Пример: 3200 МГц";

            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer", manufacturerPattern, manufacturerErrorMessage),
                    new("ManufacturerCode"),
                    new("Model"),
                    new("Socket"),
                    new("Architecture"),
                    new("CoresCount", сoresCountPattern, сoresCountErrorMessage),
                    new("ThreadsCount", threadsCountPattern, threadsCountErrorMessage),
                    new("ClockSpeed", clockSpeedPattern, clockSpeedErrorMessage)
                };

            return rules;
        }
    }
}
