using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.Settings;
using System;
using System.Threading.Tasks;

namespace PublicElections.Api.Extensions
{
    public static class StartupConfigurationExtension
    {
        public static async Task CreateApplicationAdminAsync(this IServiceProvider serviceProvider)
        {
            var adminSettings = serviceProvider.GetService<IOptions<AdminSettings>>().Value;
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string adminRole = "Admin";
            var roleExist = await roleManager.RoleExistsAsync(adminRole);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            string userName = adminSettings.UserName;
            string userPassword = adminSettings.Password;

            var sysAdmin = new ApplicationUser
            {
                UserName = userName,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true,
                Email = userName
            };

            var _user = await userManager.FindByNameAsync(userName);

            if (_user == null)
            {
                var createbridgeApiAdminUser = await userManager.CreateAsync(sysAdmin, userPassword);
                if (createbridgeApiAdminUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(sysAdmin, adminRole);
                }
            }
        }
    }
}
