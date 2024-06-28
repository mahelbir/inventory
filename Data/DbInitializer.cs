using Inventory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceScope serviceScope)
        {
            var serviceProvider = serviceScope.ServiceProvider;

            Migrate(serviceProvider);
            await DefineRoles(serviceProvider);
            await DefaultAdmin(serviceProvider);
        }

        // Migration
        private static void Migrate(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        // Tüm rolleri tanımla
        private static async Task DefineRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                if (!await roleManager.RoleExistsAsync(role.ToString()))
                {
                    await roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
            }
        }

        // Admin rolünde varsayılan kullanıcı tanımla
        private static async Task DefaultAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminUsers = await userManager.GetUsersInRoleAsync(nameof(Roles.Admin));
            if (!adminUsers.Any())
            {
                var adminPassword = "Admin123";
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    FirstName = "Sistem",
                    LastName = "Yönetici",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(adminUser, new[] { nameof(Roles.Admin), nameof(Roles.User) });
                }
            }
        }

    }
}
