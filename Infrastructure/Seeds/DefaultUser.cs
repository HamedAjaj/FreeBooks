using Domain.Constants;
using Domain.Entity;
using Infrastructure.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Domain.Entity.Helper;

namespace Infrastructure.Seeds
{
    public static class DefaultUser
    {
       
        public static async Task SeedBasicUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = Helper.UserNameBasic,
                Email = Helper.EmailBasic,
                Name = Helper.NameBasic,
                ImageUser = "04.jpg",
                ActiveUser = true,
                EmailConfirmed = true
            };
            var user = userManager.FindByEmailAsync(defaultUser.Email);
            if (user.Result == null)
            {
                await userManager.CreateAsync(defaultUser, Helper.PasswordBasic);
                await userManager.AddToRolesAsync(defaultUser, new List<string> { Helper.Roles.Basic.ToString() });
            }
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = Helper.UserName,
                Email = Helper.Email,
                Name = Helper.Name,
                ImageUser = "1.jpg",
                ActiveUser = true,
                EmailConfirmed = true
            };
            var user = userManager.FindByEmailAsync(defaultUser.Email);
            if (user.Result == null)
            {
                await userManager.CreateAsync(defaultUser, Helper.Password);
                await userManager.AddToRolesAsync(defaultUser, new List<string> { Helper.Roles.SuperAdmin.ToString() });
            }
            //Code Seeding Claims
            await roleManager.SeedClaimsAsync();
        }

        public static async Task SeedClaimsAsync(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Helper.Roles.SuperAdmin.ToString());
            // code AddPermission claims
            var modules = Enum.GetValues(typeof(PermissionsModulesName));
            foreach(var module in modules)
            {
                await roleManager.AddPermissionClaims(adminRole, module.ToString());
            }
        }

        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager,IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermission = Permissions.GeneratePermissionFromModule(module);

            foreach(var permission in allPermission)
            {
                if (!allClaims.Any(x => x.Type == Helper.Permission && x.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim(Helper.Permission, permission));
            }
        }
    }

}
