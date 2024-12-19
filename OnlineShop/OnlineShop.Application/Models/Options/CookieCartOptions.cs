using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Options
{
    public class CookieCartOptions
    {
        [Required]
        public required string CartKey { get; init; }

        [Required]
        [Range(1, 30, ErrorMessage = "Время хранения в Cookie должно быть от {1} до {2} суток")]
        public required int ExpiresDays { get; init; }
    }
}
