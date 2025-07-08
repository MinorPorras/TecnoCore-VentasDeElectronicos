// Data/DbInitializer.cs

using Microsoft.AspNetCore.Identity;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.EntityFrameworkCore; // Asegúrate de que Usuarios y Roles estén aquí

namespace Inventario_Productos_Tecnologicos.Data;

public class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuarios>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Roles>>(); // Usa tu clase Roles
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
        var context = scope.ServiceProvider.GetRequiredService<TecnoCoreDbContext>();

        // 1. Crear Roles si no existen
        string[] roleNames = { "Administrador", "Cliente" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var result = await roleManager.CreateAsync(new Roles
                    { Name = roleName, Activo = true, NormalizedName = roleName.ToUpper() });
                if (result.Succeeded)
                    logger.LogInformation($"Role '{roleName}' created successfully.");
                else
                    logger.LogError(
                        $"Error creating role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // 2. Crear Usuario Administrador si no existe
        var adminUserEmail = "admin@tecnocore.com";
        var adminPassword = "Password123!";

        var adminUser = await userManager.FindByEmailAsync(adminUserEmail);

        if (adminUser == null)
        {
            adminUser = new Usuarios
            {
                UserName = adminUserEmail,
                Email = adminUserEmail,
                NormalizedEmail = adminUserEmail.ToUpper(),
                EmailConfirmed = true, // Confirmar el email para que pueda iniciar sesión sin verificación
                Nombre = "Admin",
                Apellidos = "TecnoCore",
                Activo = true
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createAdminResult.Succeeded)
            {
                logger.LogInformation($"Admin user '{adminUserEmail}' created successfully.");
                // Asignar el rol de Administrador
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Administrador");
                if (addRoleResult.Succeeded)
                    logger.LogInformation($"Admin user '{adminUserEmail}' assigned to 'Administrador' role.");
                else
                    logger.LogError(
                        $"Error assigning 'Administrador' role to user '{adminUserEmail}': {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
            else
            {
                logger.LogError(
                    $"Error creating admin user '{adminUserEmail}': {string.Join(", ", createAdminResult.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation($"Admin user '{adminUserEmail}' already exists.");
            //Asegurarse de que el usuario admin existente tenga el rol "Administrador"
            if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Administrador");
                if (addRoleResult.Succeeded)
                    logger.LogInformation($"Admin user '{adminUserEmail}' assigned to 'Administrador' role.");
                else
                    logger.LogError(
                        $"Error assigning 'Administrador' role to existing user '{adminUserEmail}': {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
        }

        //Crear un usuario cliente 
        var clientUserEmail = "cliente@tecnocore.com";
        var clientPassword = "Password123!";

        var clientUser = await userManager.FindByEmailAsync(clientUserEmail);
        if (clientUser == null)
        {
            clientUser = new Usuarios
            {
                UserName = clientUserEmail,
                Email = clientUserEmail,
                EmailConfirmed = true,
                Nombre = "Cliente",
                Apellidos = "Ejemplo",
                Activo = true
            };

            var createClientResult = await userManager.CreateAsync(clientUser, clientPassword);
            if (createClientResult.Succeeded)
            {
                logger.LogInformation($"Client user '{clientUserEmail}' created successfully.");
                var addRoleResult = await userManager.AddToRoleAsync(clientUser, "Cliente");
                if (addRoleResult.Succeeded)
                    logger.LogInformation($"Client user '{clientUserEmail}' assigned to 'Cliente' role.");
                else
                    logger.LogError(
                        $"Error assigning 'Cliente' role to user '{clientUserEmail}': {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
            else
            {
                logger.LogError(
                    $"Error creating client user '{clientUserEmail}': {string.Join(", ", createClientResult.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation($"Client user '{clientUserEmail}' already exists.");
            if (!await userManager.IsInRoleAsync(clientUser, "Cliente"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(clientUser, "Cliente");
                if (addRoleResult.Succeeded)
                    logger.LogInformation($"Client user '{clientUserEmail}' assigned to 'Cliente' role.");
                else
                    logger.LogError(
                        $"Error assigning 'Cliente' role to existing user '{clientUserEmail}': {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
        }

        if (!context.Provincias.Any())
        {
            var provincias = new List<Provincia>
            {
                new() { Nombre = "San José" },
                new() { Nombre = "Alajuela" },
                new() { Nombre = "Cartago" },
                new() { Nombre = "Heredia" },
                new() { Nombre = "Guanacaste" },
                new() { Nombre = "Puntarenas" },
                new() { Nombre = "Limón" }
            };
            foreach (var prov in provincias)
                if (!await context.Provincias.AnyAsync(p => p.Nombre == prov.Nombre))
                    context.Provincias.Add(prov);

            await context.SaveChangesAsync();
            logger.LogInformation($"Provincias creadas.");
        }
        else
        {
            logger.LogInformation($"Provincias ya creadas saltando relleno.");
        }

        if (!context.Cantones.Any()) // Solo si no hay cantones, los agregamos
        {
            // Obtener los IDs de las provincias recién insertadas (o existentes)
            var provinciasEnDb = await context.Provincias.ToDictionaryAsync(p => p.Nombre, p => p.Id);

            var cantones = new List<Canton>
            {
                // Cantones de San José
                new() { Nombre = "San José", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Escazú", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Desamparados", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Puriscal", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Tarrazú", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Aserrí", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Mora", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Goicoechea", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Santa Ana", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Alajuelita", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Vázquez de Coronado", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Acosta", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Tibás", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Moravia", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Montes de Oca", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Turrubares", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Dota", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Curridabat", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "Pérez Zeledón", ProvinciaId = provinciasEnDb["San José"] },
                new() { Nombre = "León Cortés Castro", ProvinciaId = provinciasEnDb["San José"] },

                // Cantones de Alajuela
                new() { Nombre = "Alajuela", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "San Ramón", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Grecia", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "San Mateo", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Atenas", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Naranjo", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Palmares", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Poás", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Orotina", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "San Carlos", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Zarcero", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Sarchí", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Upala", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Los Chiles", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Guatuso", ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { Nombre = "Río Cuarto", ProvinciaId = provinciasEnDb["Alajuela"] },

                // Cantones de Cartago
                new() { Nombre = "Cartago", ProvinciaId = provinciasEnDb["Cartago"] },
                new() { Nombre = "Paraíso", ProvinciaId = provinciasEnDb["Cartago"] },
                new() { Nombre = "La Unión", ProvinciaId = provinciasEnDb["Cartago"] },
                new() { Nombre = "Jiménez", ProvinciaId = provinciasEnDb["Cartago"] },
                new() { Nombre = "Turrialba", ProvinciaId = provinciasEnDb["Cartago"] },
                new() { Nombre = "Alvarado", ProvinciaId = provinciasEnDb["Cartago"] },
                new() { Nombre = "Oreamuno", ProvinciaId = provinciasEnDb["Cartago"] },
                new() { Nombre = "El Guarco", ProvinciaId = provinciasEnDb["Cartago"] },

                // Cantones de Heredia
                new() { Nombre = "Heredia", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "Barva", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "Santo Domingo", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "Santa Bárbara", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "San Rafael", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "San Isidro", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "Belén", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "Flores", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "San Pablo", ProvinciaId = provinciasEnDb["Heredia"] },
                new() { Nombre = "Sarapiquí", ProvinciaId = provinciasEnDb["Heredia"] },

                // Cantones de Guanacaste
                new() { Nombre = "Liberia", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Nicoya", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Santa Cruz", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Bagaces", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Carrillo", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Cañas", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Abangares", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Tilarán", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Nandayure", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "La Cruz", ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { Nombre = "Hojancha", ProvinciaId = provinciasEnDb["Guanacaste"] },

                // Cantones de Puntarenas
                new() { Nombre = "Puntarenas", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Esparza", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Buenos Aires", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Montes de Oro", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Osa", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Quepos", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Golfito", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Coto Brus", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Parrita", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Corredores", ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { Nombre = "Garabito", ProvinciaId = provinciasEnDb["Puntarenas"] },

                // Cantones de Limón
                new() { Nombre = "Limón", ProvinciaId = provinciasEnDb["Limón"] },
                new() { Nombre = "Pococí", ProvinciaId = provinciasEnDb["Limón"] },
                new() { Nombre = "Siquirres", ProvinciaId = provinciasEnDb["Limón"] },
                new() { Nombre = "Talamanca", ProvinciaId = provinciasEnDb["Limón"] },
                new() { Nombre = "Matina", ProvinciaId = provinciasEnDb["Limón"] },
                new() { Nombre = "Guácimo", ProvinciaId = provinciasEnDb["Limón"] }
            };

            foreach (var c in cantones)
                // Añade un chequeo por nombre y ProvinciaId para evitar duplicados
                if (!await context.Cantones.AnyAsync(ca => ca.Nombre == c.Nombre && ca.ProvinciaId == c.ProvinciaId))
                    context.Cantones.Add(c);

            await context.SaveChangesAsync();
            logger.LogInformation("Cantones creados existosamente.");
        }
        else
        {
            logger.LogInformation("Cantones ya exitentes, saltando creación.");
        }
    }
}