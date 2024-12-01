using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.Domain.Models;
using OnlineShop.Infrastructure.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShop.WebAPI.Middleware
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtOptions _options;

        public JWTMiddleware(RequestDelegate next, IOptions<JwtOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context, UserManager<User> userManager)
        {
            var token = context.Request.Headers.Authorization.FirstOrDefault()?
                                                             .Split(" ").Last();

            if (token is not null)
            {
                await AttachAccountToContextAsync(context, token, userManager);
            }

            await _next(context);
        }

        private async Task AttachAccountToContextAsync(HttpContext context, string token, UserManager<User> userManager)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_options.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _options.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = _options.Audience,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userEmail = jwtToken.Claims.First(x => x.Type == "email").Value;

                var user = await userManager.FindByEmailAsync(userEmail);
                var userClaims = await userManager.GetClaimsAsync(user!);
                var roles = await userManager.GetRolesAsync(user!);

                var claims = new List<Claim>(userClaims);

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                context.User = new ClaimsPrincipal(identity);
            }
            catch
            {
            }
        }
    }
}
