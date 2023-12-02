using Microsoft.AspNetCore.Identity;
using ProyectoApiContable.Entities;

namespace ProyectoApiContable.Entities
{
    public static class ApplicationDbContextData
    {
        public static async Task LoadDataAsync(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            try
            {
                if (!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("Empleado"));
                }

                if (!userManager.Users.Any())
                {
                    var userAdmin = new IdentityUser
                    {
                        Email = "luisc@me.com",
                        UserName = "luisc@me.com"
                    };
                    await userManager.CreateAsync(userAdmin, "Cur0c@2020");
                    await userManager.AddToRoleAsync(userAdmin, "Admin");

                    var normalUser = new IdentityUser
                    {
                        Email = "pepito@me.com",
                        UserName = "pepito@me.com"
                    };
                    await userManager.CreateAsync(normalUser, "Cur0c@2021");
                    await userManager.AddToRoleAsync(normalUser, "Admin");
                }
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
                logger.LogError(e.Message);
            }
        }
    }
}
