using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inventario_Productos_Tecnologicos.Models.ViewModels;


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

    public IActionResult Create()
    {
        throw new NotImplementedException();
    }
}