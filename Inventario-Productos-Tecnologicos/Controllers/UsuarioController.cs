using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;


namespace Inventario_Productos_Tecnologicos.Controllers;

public class UsuarioController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly UserManager<TECO_A_Usuario> _userManager;
    private readonly RoleManager<TECO_A_Roles> _roleManager;
    private readonly ILogger<AccountController> _logger;

    public UsuarioController(
        TecnoCoreDbContext context,
        UserManager<TECO_A_Usuario> userManager,
        RoleManager<TECO_A_Roles> roleManager,
        SignInManager<TECO_A_Usuario> signInManager,
        ILogger<AccountController> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    // GET
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Debe iniciar sesión primero"));
                return RedirectToAction("Login", "Account");
            }

            // Cargar usuarios con sus relaciones
            var usuarios = await _userManager.Users
                .Include(u => u.Direccion)
                .ThenInclude(d => d.Canton)
                .ThenInclude(c => c.Provincia)
                .ToListAsync();

            // Obtener los roles para cada usuario
            var usuariosConRoles = new Dictionary<string, List<string>>();
            foreach (var user in usuarios)
            {
                var rolesUsuario = await _userManager.GetRolesAsync(user);
                usuariosConRoles[user.Id] = rolesUsuario.ToList();
            }

            // Cargar roles para el dropdown de filtrado
            var roles = await _roleManager.Roles.ToListAsync();

            // Asignar a ViewBag
            ViewBag.UsuariosRoles = usuariosConRoles;
            ViewBag.Roles = roles;
            ViewBag.Rol = "all";

            return View(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar usuarios y roles");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar los usuarios y roles"));
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Search(string searchElement, string activeFilter, string rol)
    {
        try
        {
            var query = _userManager.Users
                .Include(u => u.Direccion)
                .ThenInclude(d => d.Canton)
                .ThenInclude(c => c.Provincia)
                .AsQueryable();

            // Aplicar filtro de búsqueda si existe
            if (!string.IsNullOrEmpty(searchElement))
                query = query.Where(u => u.Email.Contains(searchElement) ||
                                         u.UserName.Contains(searchElement));

            // Aplicar filtro de estado si no es "all"
            if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
            {
                var isActive = activeFilter == "true";
                query = query.Where(u => u.TB_Activo == isActive);
            }

            var usuarios = await query.ToListAsync();

            // Obtener los roles para cada usuario
            var usuariosConRoles = new Dictionary<string, List<string>>();
            foreach (var user in usuarios)
            {
                var rolesUsuario = await _userManager.GetRolesAsync(user);
                usuariosConRoles[user.Id] = rolesUsuario.ToList();
            }

            // Si se seleccionó un rol específico, filtrar por ese rol
            if (!string.IsNullOrEmpty(rol) && rol != "all")
                usuarios = usuarios.Where(u =>
                    usuariosConRoles.ContainsKey(u.Id) &&
                    usuariosConRoles[u.Id].Contains(rol)).ToList();

            // Cargar roles para el dropdown de filtrado
            var roles = await _roleManager.Roles.ToListAsync();

            // Asignar a ViewBag
            ViewBag.UsuariosRoles = usuariosConRoles;
            ViewBag.Roles = roles;
            ViewBag.Rol = rol ?? "all";
            ViewBag.SearchString = searchElement;
            ViewBag.ActiveFilter = activeFilter;

            return View("Index", usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar usuarios");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al buscar usuarios"));
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult Create()
    {
        try
        {
            var roles = _roleManager.Roles.Where(r => r.TB_Activo).ToList();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            var model = new RegisterViewModel
            {
                Provincias =
                    [..new SelectList(_context.TECO_M_Provincia.OrderBy(p => p.TC_Nombre), "TN_Id", "TC_Nombre")],
                Cantones = new List<SelectListItem>(new SelectList(Enumerable.Empty<TECO_M_Canton>(), "TN_Id",
                    "TC_Nombre"))
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el formulario de creación de usuario");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar el formulario de registro"));
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(
        [Bind(
            "UserName,Email,Password,ConfirmPassword,Rol,Nombre,Apellidos,PhoneNumber,SelectedProvinciaId,SelectedCantonId,DireccionExacta,CodigoPostal")]
        RegisterViewModel model)
    {
        _logger.LogInformation("Iniciando creación de usuario");

        try
        {
            if (!ModelState.IsValid)
            {
                // Recolectar todos los mensajes de error
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Crear un mensaje combinado con todos los errores
                var errorMessage = string.Join("\n• ", errorMessages);
                errorMessage = "Se encontraron los siguientes errores:\n• " + errorMessage;

                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert(errorMessage));

                // Recargar los datos necesarios para el formulario
                var roles = _roleManager.Roles.Where(r => r.TB_Activo).ToList();
                ViewBag.Roles = new SelectList(roles, "Id", "Name");
                model.Provincias =
                    new List<SelectListItem>(new SelectList(_context.TECO_M_Provincia.OrderBy(p => p.TC_Nombre),
                        "TN_Id", "TC_Nombre"));
                if (model.SelectedProvinciaId > 0)
                    model.Cantones = new List<SelectListItem>(new SelectList(
                        _context.TECO_M_Canton.Where(c => c.TN_ProvinciaId == model.SelectedProvinciaId),
                        "TN_Id", "TC_Nombre"));
                else
                    model.Cantones =
                        new List<SelectListItem>(
                            new SelectList(Enumerable.Empty<TECO_M_Canton>(), "TN_Id", "TC_Nombre"));

                return View(model);
            }

            var usuario = new TECO_A_Usuario
            {
                UserName = model.UserName,
                Email = model.Email,
                TC_Nombre = model.Nombre,
                TC_Apellidos = model.Apellidos,
                PhoneNumber = model.PhoneNumber,
                TB_Activo = true
            };

            var direccion = new TECO_A_Direccion
            {
                TC_Direccion = model.DireccionExacta,
                TC_CodigoPostal = model.CodigoPostal,
                TN_CantonId = model.SelectedCantonId ?? 0
            };

            usuario.Direccion = direccion;

            var result = await _userManager.CreateAsync(usuario, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.Rol))
                {
                    var role = await _roleManager.FindByIdAsync(model.Rol);
                    if (role != null) await _userManager.AddToRoleAsync(usuario, role.Name ?? string.Empty);
                }

                TempData["success"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.SuccessAlert());
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

            // Si llegamos aquí, algo falló, recargamos las listas
            var rolesList = _roleManager.Roles.Where(r => r.TB_Activo).ToList();
            ViewBag.Roles = new SelectList(rolesList, "Id", "Name");
            model.Provincias =
                new List<SelectListItem>(new SelectList(_context.TECO_M_Provincia.OrderBy(p => p.TC_Nombre), "TN_Id",
                    "TC_Nombre"));
            model.Cantones = new List<SelectListItem>(new SelectList(
                _context.TECO_M_Canton.Where(c => c.TN_ProvinciaId == model.SelectedProvinciaId),
                "TN_Id", "TC_Nombre"));

            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Por favor, revise los datos ingresados"));
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert($"Error al crear el usuario: {ex.Message}"));
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> SwitchActive(string id)
    {
        try
        {
            var usuario = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el producto"));
                return RedirectToAction(nameof(Index));
            }

            //Se verifica el estado de la dirección para que calce con el estado del usuario
            var direccion = await _context.TECO_A_Direccion
                .FirstOrDefaultAsync(d => d.TN_UsuarioId == usuario.Id);
            _logger.LogCritical("Estado del usuario: {EstadoUsuario}, Estado de la dirección: {EstadoDireccion}",
                usuario.TB_Activo, direccion?.TB_Activo);
            if (usuario.TB_Activo)
            {
                if (direccion is { TB_Activo: true })
                {
                    //Si el usuario está activo va a ser desacticado por lo que se desactiva la dirección
                    direccion.TB_Activo = false;
                    _context.TECO_A_Direccion.Update(direccion);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                if (direccion is { TB_Activo: false })
                {
                    //Si el usuario está inactivo va a ser activado por lo que se activa su dirección
                    direccion.TB_Activo = true;
                    _context.TECO_A_Direccion.Update(direccion);
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogCritical("Estado del usuario: {EstadoUsuario}, Estado de la dirección: {EstadoDireccion}",
                usuario.TB_Activo, direccion?.TB_Activo);

            //Se revisa si el usuario es el mismo que el usuario autenticado
            if (usuario.UserName == User.Identity?.Name)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("No puede cambiar el estado de su propio usuario"));
                return RedirectToAction(nameof(Index));
            }

            //Se actualiza el estado del usuario
            usuario.TB_Activo = !usuario.TB_Activo;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.InfoAlert("Estado del producto cambiado correctamente"));
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cambiar el estado del producto"));
            _logger.LogError(ex, "Error al cambiar el estado del producto {ProductoId}", id);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var usuario = _userManager.Users
                .Include(u => u.Direccion)
                .ThenInclude(d => d.Canton)
                .ThenInclude(c => c.Provincia)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el usuario"));
                return RedirectToAction(nameof(Index));
            }

            var rolesActivos = _roleManager.Roles.Where(r => r.TB_Activo).ToList();

            // Obtener el nombre del rol actual del usuario
            var userRolesNames =
                await _userManager
                    .GetRolesAsync(usuario); // Asegúrate de que este método sea async si el controlador no lo es
            var userCurrentRoleName = userRolesNames.FirstOrDefault();

            // Encontrar el ID del rol actual del usuario
            string userRoleId = null;
            if (!string.IsNullOrEmpty(userCurrentRoleName))
            {
                var userRole = rolesActivos.FirstOrDefault(r => r.Name == userCurrentRoleName);
                if (userRole != null) userRoleId = userRole.Id;
            }

            // Crear el SelectList para la vista. El campo "Id" del rol será el valor, y "Name" será el texto.
            // Aquí no se pasa el valor seleccionado aún, se hará en la vista con asp-for.
            ViewBag.Roles = new SelectList(rolesActivos, "Id", "Name");


            _logger.LogCritical($"Rol del usuario: " + userCurrentRoleName); // Puedes mantener esto para depuración
            var model = new EditUserViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName,
                Email = usuario.Email,
                Nombre = usuario.TC_Nombre,
                Apellidos = usuario.TC_Apellidos,
                PhoneNumber = usuario.PhoneNumber,
                SelectedProvinciaId = usuario.Direccion?.Canton?.Provincia?.TN_Id ?? 0,
                SelectedCantonId = usuario.Direccion?.TN_CantonId ?? 0,
                DireccionExacta = usuario.Direccion?.TC_Direccion ?? string.Empty,
                CodigoPostal = usuario.Direccion?.TC_CodigoPostal ?? string.Empty,
                // AQUÍ ESTÁ EL CAMBIO CLAVE: Asigna el ID del rol, no el nombre
                Rol = userRoleId, // Asigna el ID del rol que acabamos de encontrar
                Provincias =
                    [..new SelectList(_context.TECO_M_Provincia.OrderBy(p => p.TC_Nombre), "TN_Id", "TC_Nombre")],
                Cantones = new List<SelectListItem>(new SelectList(Enumerable.Empty<TECO_M_Canton>(), "TN_Id",
                    "TC_Nombre"))
            };

            if (model.SelectedProvinciaId > 0)
                model.Cantones =
                    new List<SelectListItem>(new SelectList(_context.TECO_M_Canton
                        .Where(c => c.TN_ProvinciaId == model.SelectedProvinciaId), "TN_Id", "TC_Nombre"));

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el formulario de edición de usuario");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar el formulario de edición de usuario"));
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit([FromBody] EditUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Recolectar todos los mensajes de error
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Crear un mensaje combinado con todos los errores
                var errorMessage = string.Join("\n• ", errorMessages);
                errorMessage = "Se encontraron los siguientes errores:\n• " + errorMessage;

                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert(errorMessage));

                // Recargar los datos necesarios para el formulario
                reloadFormElements(model);
                return View("Edit", model);
            }

            //Busca el usuario y si no lo encuentra devuelve un mensaje de error
            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el usuario"));

                return View(model);
            }

            _logger.LogCritical("Usuario encontrado: {UserName}", usuario.UserName);

            usuario.UserName = model.UserName;
            usuario.Email = model.Email;
            usuario.TC_Nombre = model.Nombre;
            usuario.TC_Apellidos = model.Apellidos;
            usuario.PhoneNumber = model.PhoneNumber;

            //Actualizar el usuario
            var result = await _userManager.UpdateAsync(usuario);

            //Si el usuario se actualiza correctamente actualiza la demás información sino devuelve la vista de edición
            if (result.Succeeded)
            {
                // Actualizar el rol del usuario
                if (!string.IsNullOrEmpty(model.Rol))
                {
                    var role = await _roleManager.FindByIdAsync(model.Rol);
                    if (role != null)
                    {
                        // Primero eliminamos todos los roles del usuario
                        var currentRoles = await _userManager.GetRolesAsync(usuario);
                        await _userManager.RemoveFromRolesAsync(usuario, currentRoles);

                        // Luego agregamos el nuevo rol
                        await _userManager.AddToRoleAsync(usuario, role.Name ?? string.Empty);
                    }
                    else
                    {
                        _logger.LogCritical("Rol no encontrado: {Rol}", model.Rol);
                    }
                }

                // Actualizar la dirección del usuario
                var direccion = await _context.TECO_A_Direccion
                    .FirstOrDefaultAsync(d => d.TN_UsuarioId == usuario.Id);
                _logger.LogCritical("Dirección encontrada: {DireccionId}", direccion?.TN_Id);
                _logger.LogCritical("Cantón seleccionado: {SelectedCantonId}", model.SelectedCantonId);
                if (direccion == null)
                {
                    // Si la dirección NO existe, la CREAMOS y la agregamos al contexto
                    direccion = new TECO_A_Direccion
                    {
                        TN_UsuarioId = usuario.Id, // <-- ¡IMPORTANTÍSIMO! Asigna el ID del usuario
                        TB_Activo = true,
                        TC_Direccion = model.DireccionExacta,
                        TC_CodigoPostal = model.CodigoPostal,
                        TN_CantonId = model.SelectedCantonId ?? 0
                    };
                    _context.TECO_A_Direccion.Add(direccion); // <-- Usar Add para nuevas entidades
                }
                else
                {
                    direccion.TC_Direccion = model.DireccionExacta;
                    direccion.TC_CodigoPostal = model.CodigoPostal;
                    direccion.TN_CantonId = model.SelectedCantonId ?? 0;
                }

                await _context.SaveChangesAsync();
                TempData["success"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.SuccessAlert());
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Error al actualizar el usuario: " +
                                     string.Join(", ", result.Errors.Select(e => e.Description))));

                // Recargar los datos necesarios para el formulario
                reloadFormElements(model);
                return View(model);
            }
        }
        catch (Exception ex)
        {
            reloadFormElements(model);
            _logger.LogError(ex, "Error al editar usuario");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert($"Error al editar el usuario: {ex.Message}"));
            return View(model);
        }
    }

    
    private void reloadFormElements(EditUserViewModel model)
    {
        try
        {
            // Recargar los datos necesarios para el formulario
            var roles = _roleManager.Roles.Where(r => r.TB_Activo).ToList();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            model.Provincias =
                new List<SelectListItem>(new SelectList(_context.TECO_M_Provincia.OrderBy(p => p.TC_Nombre),
                    "TN_Id",
                    "TC_Nombre"));
            if (model.SelectedProvinciaId > 0)
                model.Cantones =
                [
                    ..new SelectList(
                        _context.TECO_M_Canton.Where(c => c.TN_ProvinciaId == model.SelectedProvinciaId),
                        "TN_Id", "TC_Nombre")
                ];
            else
                model.Cantones =
                [
                    ..new SelectList(Enumerable.Empty<TECO_M_Canton>(), "TN_Id", "TC_Nombre")
                ];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public IActionResult Informacion_personal()
    {
        try
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            if (currentUserId == null)
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert($"Id de Usuario no encontrado"));
            var usuario = _userManager.Users
                .Include(u => u.Direccion)
                .ThenInclude(d => d.Canton)
                .ThenInclude(c => c.Provincia)
                .FirstOrDefault(u => u.Id == currentUserId);
            if (usuario == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert($"Usuario no encontrado"));
                return View("Index");
            }

            var model = new RegisterViewModel
            {
                UserName = usuario.UserName ?? string.Empty,
                Email = usuario.Email ?? "No agregado",
                Nombre = usuario.TC_Nombre,
                Apellidos = usuario.TC_Apellidos,
                PhoneNumber = usuario.PhoneNumber ?? "No agregado",
                DireccionExacta = usuario.Direccion?.TC_Direccion ?? "No agregado",
                CodigoPostal = usuario.Direccion?.TC_CodigoPostal ?? "No agregado"
            };
            ViewBag.ProvinciaName = usuario.Direccion?.Canton?.Provincia?.TC_Nombre ?? "No disponible";
            ViewBag.CantonName = usuario.Direccion?.Canton?.TC_Nombre ?? "No disponible";
            return View(model);
        }
        catch (Exception e)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert($"Error al cargar la información personal: {e.Message}"));
            _logger.LogError(e.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}