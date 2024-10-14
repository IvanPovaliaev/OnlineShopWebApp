﻿using OnlineShopWebApp.Interfaces;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class PowerSupplySpecificationsRules : IProductSpecificationsRules
    {
        public List<ProductSpecificationRule> GetAll()
        {
            var powerPattern = @"^\d+\sВт$";
            var powerPatternErrorMessage = @"Неверная мощность. Пример: 600 Вт";

            var pfcPattern = @"^(активный|пассивный)$";
            var pfcErrorMessage = @"Неверный тип. Доступные варианты: активный; пассивный";

            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("Power", powerPattern, powerPatternErrorMessage),
                    new("PFC", pfcPattern, pfcErrorMessage),
                    new("FanSize")
                };

            return rules;
        }
    }
}