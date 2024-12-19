using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Infrastructure.Email
{
    public class MailSettings
    {
        [Required]
        public required string Mail { get; init; }

        [Required]
        public required string DisplayName { get; init; }

        [Required]
        public required string Password { get; init; }

        [Required]
        public string? Host { get; init; }

        [Required]
        public int Port { get; init; }
    }
}
