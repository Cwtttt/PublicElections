using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PublicElections.Domain.Entities;
using PublicElections.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicElections.Api.Extensions
{
    public static class StartupConfigurationExtension
    {
        public static async Task CreateApplicationAdminAsync(this IServiceProvider serviceProvider)
        {
                var adminSettings = serviceProvider.GetService<IOptions<AdminSettings>>().Value;
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string AdminRole = "Admin";
                var roleExist = await RoleManager.RoleExistsAsync(AdminRole);
                if (!roleExist)
                {
                    await RoleManager.CreateAsync(new IdentityRole(AdminRole));
                }
                string userName = adminSettings.UserName;
                string userPassword = adminSettings.Password;

                var sysAdmin = new ApplicationUser
                {
                    UserName = userName,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var _user = await UserManager.FindByNameAsync(userName);

                if (_user == null)
                {
                    var createbridgeApiAdminUser = await UserManager.CreateAsync(sysAdmin, userPassword);
                    if (createbridgeApiAdminUser.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(sysAdmin, AdminRole);
                    }
                }
        }
    }
}
