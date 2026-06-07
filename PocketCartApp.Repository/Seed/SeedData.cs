using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.Identity_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Repository.Seed
{
    public class SeedData
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Cashier"))
            {
                await roleManager.CreateAsync(new IdentityRole("Cashier"));
            }
        }

        public static async Task SeedAdmin(IServiceProvider serviceProvider)
        {
            var userManager =
                serviceProvider.GetRequiredService<UserManager<PocketCartApplicationUser>>();

            var adminEmail = "admin@pocketcart.com";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                var newAdmin = new PocketCartApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Main",
                    EmployeeId = "EMP -" + DateTime.Now.Year + "-001",
                    StartDate = DateTime.Now,
                    contract_Type = Contract_Type.Permanent,
                    cashierId = "112233",
                    Account_Status = Account_Status.Active,
                    LastName = "Admin"
                };

                await userManager.CreateAsync(newAdmin, "Admin123!");

                await userManager.AddToRoleAsync(newAdmin, "Admin");

                if (newAdmin.ShoppingCart == null)
                {
                    newAdmin.ShoppingCart = new ShoppingCart();

                    await userManager.UpdateAsync(newAdmin);
                }
            }
        }
    }
}
