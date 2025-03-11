using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Web.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser superAdmin = new()
            {
                UserName = "SuperAdmin",
                FullName = "SuperAdmin",
                ImageUrl = "/assets/images/avatar.png",
                ImageThumbnailUrl = "/assets/images/avatar.png",
                IsDeleted = false,
                password = "P@ssword123"
            };

            var user = await userManager.FindByNameAsync(superAdmin.UserName);

            if (user is null)
            {
                await userManager.CreateAsync(superAdmin, "P@ssword123");
                var roles = roleManager.Roles.Select(r => r.Name).ToList();
                await userManager.AddToRolesAsync(superAdmin, roles);
            }
        }
    }
}