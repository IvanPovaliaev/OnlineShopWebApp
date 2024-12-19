using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Helpers
{
    public class ConfigCatOptions
    {
        [Required]
        public string Key { get; init; }

        [Required]
        public uint PollIntervalInSeconds { get; init; }
    }
}
