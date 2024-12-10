using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Infrastructure.Jwt
{
    public class JwtOptions
    {
        [Required]
        public required string SecretKey { get; init; }
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;

        [Required]
        [Range(1, 30, ErrorMessage = "Время действия токена должно быть от {1} до {2} часов")]
        public int ExpiresHours { get; init; }
    }
}