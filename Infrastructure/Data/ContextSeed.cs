using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class ContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "SuperAdmin", "Admin", "User", "Provider" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var superAdmin = new ApplicationUser
            {
                UserName = "superadmin@platform.com",
                Email = "superadmin@platform.com",
                FullName = "System Super Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.UserName != superAdmin.UserName))
            {
                var result = await userManager.CreateAsync(superAdmin, "P@ssw0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                    // If i want to make admin has the same permissions as super admin
                    // await userManager.AddToRoleAsync(superAdmin, "Admin");
                }
            }
        }
    }
}
