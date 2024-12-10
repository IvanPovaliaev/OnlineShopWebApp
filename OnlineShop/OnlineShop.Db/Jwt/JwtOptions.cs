using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Infrastructure.Jwt
{
    public class JwtOptions
    {
        [Required]
        public required string SecretKey { get; init; }
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int ExpiresHours { get; init; }
    }
}