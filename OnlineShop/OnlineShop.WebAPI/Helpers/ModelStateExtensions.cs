using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineShop.WebAPI.Helpers
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrors(this ModelStateDictionary dictionary)
        {
            return dictionary.Values
                             .SelectMany(v => v.Errors)
                             .Select(e => e.ErrorMessage)
                             .ToList();
        }
    }
}
