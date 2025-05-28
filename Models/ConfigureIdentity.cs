using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace OpenTableApp.Models
{
    public static class ConfigureIdentity
    {
        public static async Task CreateAdminUserAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string adminRole = "Admin";
            string userRole = "User";
            string adminEmail = "admin@email.com";
            string adminPassword = "Admin@123";

            //  Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            //  Ensure User role exists
            if (!await roleManager.RoleExistsAsync(userRole))
            {
                await roleManager.CreateAsync(new IdentityRole(userRole));
            }

            //  Check if admin user exists
            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                // Create new admin user
                user = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    DOB = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
            else
            {
                //  Add role if not already assigned
                if (!await userManager.IsInRoleAsync(user, adminRole))
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
        }
    }
}
