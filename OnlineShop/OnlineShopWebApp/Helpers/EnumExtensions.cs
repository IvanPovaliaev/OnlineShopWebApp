using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace OnlineShopWebApp.Helpers
{
    public static class EnumExtensions
    {
        /// <summary>
        /// A extensions method that return Name value of Display attribute
        /// </summary>
        /// <returns>DisplayAttribute Name value (string)</returns>
        /// <param name="enumValue">Target enum</param>
        public static string GetDisplayAttributeName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()!
                .GetName()!;
        }
    }
}
