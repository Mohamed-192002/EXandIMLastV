using EXandIM.Web.Core;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Web.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.SuperAdmin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.User));

                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanCreateExport));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanEditExport));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanViewExport));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanDeleteExport));

                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanCreateImport));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanEditImport));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanViewImport));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanDeleteImport));

                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanCreateReading));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanEditReading));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanViewReading));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanDeleteReading));

                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanCreateActivity));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanEditActivity));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanViewActivity));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanDeleteActivity));

                await roleManager.CreateAsync(new IdentityRole(AppRoles.CanViewMyTeamOnly));
            }
            
        }
    }
}