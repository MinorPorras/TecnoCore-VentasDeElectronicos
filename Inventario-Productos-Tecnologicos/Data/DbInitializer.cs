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
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TECO_A_Usuario>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TECO_A_Roles>>(); // Usa tu clase Roles
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
        var context = scope.ServiceProvider.GetRequiredService<TecnoCoreDbContext>();

        // 1. Crear Roles si no existen
        string[] roleNames = { "Administrador", "Cliente" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var result = await roleManager.CreateAsync(new TECO_A_Roles
                    { Name = roleName, TB_Activo = true, NormalizedName = roleName.ToUpper() });
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
            adminUser = new TECO_A_Usuario
            {
                UserName = adminUserEmail,
                Email = adminUserEmail,
                NormalizedEmail = adminUserEmail.ToUpper(),
                EmailConfirmed = true,
                TC_Nombre = "Admin",
                TC_Apellidos = "TecnoCore",
                TB_Activo = true
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createAdminResult.Succeeded)
            {
                logger.LogInformation($"Admin user '{adminUserEmail}' created successfully.");

                // Obtener la provincia y cantón de San José
                var provincia = await context.TECO_M_Provincia.FirstOrDefaultAsync(p => p.TC_Nombre == "San José");
                var canton = await context.TECO_M_Canton.FirstOrDefaultAsync(c =>
                    c.TC_Nombre == "San José" && c.TN_ProvinciaId == provincia.TN_Id);

                // Crear y asignar dirección
                var direccionAdmin = new TECO_A_Direccion
                {
                    TC_Direccion = "Dirección Administrativa TecnoCore",
                    TC_CodigoPostal = "10101",
                    TN_CantonId = canton.TN_Id,
                    TN_UsuarioId = adminUser.Id,
                    TB_Activo = true
                };

                context.TECO_A_Direccion.Add(direccionAdmin);
                await context.SaveChangesAsync();

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
            clientUser = new TECO_A_Usuario
            {
                UserName = clientUserEmail,
                Email = clientUserEmail,
                EmailConfirmed = true,
                TC_Nombre = "Cliente",
                TC_Apellidos = "Ejemplo",
                TB_Activo = true
            };

            var createClientResult = await userManager.CreateAsync(clientUser, clientPassword);
            if (createClientResult.Succeeded)
            {
                logger.LogInformation($"Client user '{clientUserEmail}' created successfully.");

                // Obtener la provincia y cantón de San José
                var provincia = await context.TECO_M_Provincia.FirstOrDefaultAsync(p => p.TC_Nombre == "San José");
                var canton = await context.TECO_M_Canton.FirstOrDefaultAsync(c =>
                    c.TC_Nombre == "San José" && c.TN_ProvinciaId == provincia.TN_Id);

                // Crear y asignar dirección
                var direccionCliente = new TECO_A_Direccion
                {
                    TC_Direccion = "Dirección Cliente TecnoCore",
                    TC_CodigoPostal = "10101",
                    TN_CantonId = canton.TN_Id,
                    TN_UsuarioId = clientUser.Id,
                    TB_Activo = true
                };

                context.TECO_A_Direccion.Add(direccionCliente);
                await context.SaveChangesAsync();

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

        if (!context.TECO_M_Provincia.Any())
        {
            var provincias = new List<TECO_M_Provincia>
            {
                new() { TC_Nombre = "San José" },
                new() { TC_Nombre = "Alajuela" },
                new() { TC_Nombre = "Cartago" },
                new() { TC_Nombre = "Heredia" },
                new() { TC_Nombre = "Guanacaste" },
                new() { TC_Nombre = "Puntarenas" },
                new() { TC_Nombre = "Limón" }
            };
            foreach (var prov in provincias)
                if (!await context.TECO_M_Provincia.AnyAsync(p => p.TC_Nombre == prov.TC_Nombre))
                    context.TECO_M_Provincia.Add(prov);

            await context.SaveChangesAsync();
            logger.LogInformation($"Provincias creadas.");
        }
        else
        {
            logger.LogInformation($"Provincias ya creadas saltando relleno.");
        }

        if (!context.TECO_M_Canton.Any()) // Solo si no hay cantones, los agregamos
        {
            // Obtener los IDs de las provincias recién insertadas (o existentes)
            var provinciasEnDb = await context.TECO_M_Provincia.ToDictionaryAsync(p => p.TC_Nombre, p => p.TN_Id);

            var cantones = new List<TECO_M_Canton>
            {
                // Cantones de San José
                new() { TC_Nombre = "San José", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Escazú", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Desamparados", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Puriscal", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Tarrazú", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Aserrí", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Mora", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Goicoechea", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Santa Ana", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Alajuelita", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Vázquez de Coronado", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Acosta", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Tibás", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Moravia", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Montes de Oca", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Turrubares", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Dota", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Curridabat", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "Pérez Zeledón", TN_ProvinciaId = provinciasEnDb["San José"] },
                new() { TC_Nombre = "León Cortés Castro", TN_ProvinciaId = provinciasEnDb["San José"] },

                // Cantones de Alajuela
                new() { TC_Nombre = "Alajuela", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "San Ramón", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Grecia", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "San Mateo", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Atenas", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Naranjo", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Palmares", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Poás", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Orotina", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "San Carlos", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Zarcero", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Sarchí", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Upala", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Los Chiles", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Guatuso", TN_ProvinciaId = provinciasEnDb["Alajuela"] },
                new() { TC_Nombre = "Río Cuarto", TN_ProvinciaId = provinciasEnDb["Alajuela"] },

                // Cantones de Cartago
                new() { TC_Nombre = "Cartago", TN_ProvinciaId = provinciasEnDb["Cartago"] },
                new() { TC_Nombre = "Paraíso", TN_ProvinciaId = provinciasEnDb["Cartago"] },
                new() { TC_Nombre = "La Unión", TN_ProvinciaId = provinciasEnDb["Cartago"] },
                new() { TC_Nombre = "Jiménez", TN_ProvinciaId = provinciasEnDb["Cartago"] },
                new() { TC_Nombre = "Turrialba", TN_ProvinciaId = provinciasEnDb["Cartago"] },
                new() { TC_Nombre = "Alvarado", TN_ProvinciaId = provinciasEnDb["Cartago"] },
                new() { TC_Nombre = "Oreamuno", TN_ProvinciaId = provinciasEnDb["Cartago"] },
                new() { TC_Nombre = "El Guarco", TN_ProvinciaId = provinciasEnDb["Cartago"] },

                // Cantones de Heredia
                new() { TC_Nombre = "Heredia", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "Barva", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "Santo Domingo", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "Santa Bárbara", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "San Rafael", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "San Isidro", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "Belén", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "Flores", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "San Pablo", TN_ProvinciaId = provinciasEnDb["Heredia"] },
                new() { TC_Nombre = "Sarapiquí", TN_ProvinciaId = provinciasEnDb["Heredia"] },

                // Cantones de Guanacaste
                new() { TC_Nombre = "Liberia", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Nicoya", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Santa Cruz", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Bagaces", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Carrillo", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Cañas", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Abangares", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Tilarán", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Nandayure", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "La Cruz", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },
                new() { TC_Nombre = "Hojancha", TN_ProvinciaId = provinciasEnDb["Guanacaste"] },

                // Cantones de Puntarenas
                new() { TC_Nombre = "Puntarenas", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Esparza", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Buenos Aires", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Montes de Oro", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Osa", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Quepos", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Golfito", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Coto Brus", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Parrita", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Corredores", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },
                new() { TC_Nombre = "Garabito", TN_ProvinciaId = provinciasEnDb["Puntarenas"] },

                // Cantones de Limón
                new() { TC_Nombre = "Limón", TN_ProvinciaId = provinciasEnDb["Limón"] },
                new() { TC_Nombre = "Pococí", TN_ProvinciaId = provinciasEnDb["Limón"] },
                new() { TC_Nombre = "Siquirres", TN_ProvinciaId = provinciasEnDb["Limón"] },
                new() { TC_Nombre = "Talamanca", TN_ProvinciaId = provinciasEnDb["Limón"] },
                new() { TC_Nombre = "Matina", TN_ProvinciaId = provinciasEnDb["Limón"] },
                new() { TC_Nombre = "Guácimo", TN_ProvinciaId = provinciasEnDb["Limón"] }
            };

            foreach (var c in cantones)
                // Añade un chequeo por nombre y TN_ProvinciaId para evitar duplicados
                if (!await context.TECO_M_Canton.AnyAsync(ca =>
                        ca.TC_Nombre == c.TC_Nombre && ca.TN_ProvinciaId == c.TN_ProvinciaId))
                    context.TECO_M_Canton.Add(c);

            await context.SaveChangesAsync();
            logger.LogInformation("Cantones creados existosamente.");
        }
        else
        {
            logger.LogInformation("Cantones ya exitentes, saltando creación.");
        }
    }
}