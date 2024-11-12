using Microsoft.AspNetCore.Identity;
using OnlineShop.Db.Models;
using System.Threading.Tasks;

namespace OnlineShop.Db
{
    public class IdentityInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var adminEmail = "PCDdream@gmail.com";
            var password = "Qwerty123!";

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

            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail
                };
                var result = await userManager.CreateAsync(admin, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Constants.AdminRoleName);
                }
            }
        }
    }
}
