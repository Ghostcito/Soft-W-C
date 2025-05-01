using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Soft_W_C.Models;

namespace Soft_W_C.Data
{
    public static class IdentityDataInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
       
        string[] roles = new[] { "Administrador", "Empleado" };

        // Crear los roles si no existen
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Crear el usuario con rol admin si no existe
        string roleName = "Administrador";
        string adminEmail = "admin@gmail.com";
        string adminPassword = "Admin123*@";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = "Administradorwc",
                Email = adminEmail,
                EmailConfirmed = true,
                Nombre = "Administrador wc",
                Apellido = "Sistema",
                NivelAcceso = "3", // Nivel de acceso para el administrador varia en 1 - 2 - 3
                Estado = "activo",
                DNI = "12345678"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, roleName);   
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Code} - {error.Description}");
                }
            }
        }
    }
    }
}