using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Application.Services
{
    public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private readonly HashService _hashService;

        public Argon2PasswordHasher(HashService hashService)
        {
            _hashService = hashService;
        }
        public string HashPassword(TUser user, string password)
        {
            return _hashService.GenerateHash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            return _hashService.IsEquals(providedPassword, hashedPassword)
                                ? PasswordVerificationResult.Success
                                : PasswordVerificationResult.Failed;
        }
    }
}
