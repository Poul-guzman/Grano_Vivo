using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using SolucionWebGranoVivo.Models;
using Microsoft.EntityFrameworkCore; 

namespace SolucionWebGranoVivo.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            

            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            string[] roles = { "Administrador", "Cliente", "Trabajador" };

            

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        Descripcion = $"Rol por defecto: {roleName}"
                    };

                    await roleManager.CreateAsync(role);
                }
            }

            

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string adminEmail = "admin@granovivo.com";
            string adminPassword = "Admin123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nombre = "Administrador",
                    Apellidos = "General",
                    DNI = "00000000",
                    Telefono = "999999999",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }

            string trabajadorEmail = "trabajador@granovivo.com";
            string trabajadorPassword = "Trabajador123";

            var trabajadorUser = await userManager.FindByEmailAsync(trabajadorEmail);
            if (trabajadorUser == null)
            {
                trabajadorUser = new ApplicationUser
                {
                    UserName = trabajadorEmail,
                    Email = trabajadorEmail,
                    Nombre = "Juan",
                    Apellidos = "Pérez",
                    DNI = "12345678",
                    Telefono = "987654321",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(trabajadorUser, trabajadorPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(trabajadorUser, "Trabajador");
                }
            }
        }
    }
}