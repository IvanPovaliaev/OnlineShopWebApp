using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OnlineShop.Db.Models;
using System.Threading.Tasks;

namespace OnlineShop.Db
{
    public class IdentityInitializer
    {
        private readonly IConfiguration _configuration;

        public IdentityInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InitializeAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (await roleManager.FindByNameAsync(Constants.AdminRoleName) is null)
            {
                var adminRole = new Role()
                {
                    Name = Constants.AdminRoleName,
                    CanBeDeleted = false
                };

                await roleManager.CreateAsync(adminRole);
            }

            if (await roleManager.FindByNameAsync(Constants.UserRoleName) is null)
            {
                var userRole = new Role()
                {
                    Name = Constants.UserRoleName,
                    CanBeDeleted = false
                };

                await roleManager.CreateAsync(userRole);
            }

            var adminEmail = _configuration["AdminSettings:AdminEmail"];

            if (await userManager.FindByEmailAsync(adminEmail!) is null)
            {
                var admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail
                };

                var adminPassword = _configuration["AdminSettings:AdminPassword"];

                var result = await userManager.CreateAsync(admin, adminPassword!);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Constants.AdminRoleName);
                }
            }
        }
    }
}
