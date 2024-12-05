using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineShop.WebAPI.Helpers
{
	public static class ModelStateExtensions
	{
		/// <summary>
		/// Get collection of all error from current ModelStateDictionary
		/// </summary>
		/// <returns>Collection of error messages</returns>
		public static List<string> GetErrors(this ModelStateDictionary dictionary)
		{
			return dictionary.Values
							 .SelectMany(v => v.Errors)
							 .Select(e => e.ErrorMessage)
							 .ToList();
		}
	}
}
