using TicketControl.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketControl.Data.Models;

namespace TicketControl.Web
{
    public static class SeedData
    {
        public static void Seed(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if(userManager.FindByNameAsync("admin").Result == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "admin@localhost.com",
                    
                };
                var result = userManager.CreateAsync(user, "password").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                var result = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Developer").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Developer"
                };
                var result = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
