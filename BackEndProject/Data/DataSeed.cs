using BackEndProject.Entity;
using Microsoft.AspNetCore.Identity;

namespace BackEndProject.Data
{
    public class DataSeed
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            #region Roles

            var roles = new string[] { "Admin", "Moderator" };
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in roles)
            {
                var existingRole = await roleManager.FindByNameAsync(role);
                if (existingRole != null) continue;
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            #endregion

            #region Admin Creation
            string adminUserName = (string)configuration["DefaultAdmin:UserName"]!;
            string adminPassword = (string)configuration["DefaultAdmin:Password"]!;

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var admin = await userManager.FindByNameAsync(adminUserName);

            if (admin != null) return;

            admin = new AppUser
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = adminUserName
            };

            await userManager.CreateAsync(admin, adminPassword);

            await userManager.AddToRoleAsync(admin, roles[0]);
            #endregion
        }
    }
}
