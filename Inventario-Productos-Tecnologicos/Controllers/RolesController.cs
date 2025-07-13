using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class RolesController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<RolesController> _logger;
    private readonly RoleManager<TECO_A_Roles> _roleManager;

    public RolesController(TecnoCoreDbContext context, ILogger<RolesController> logger,
        RoleManager<TECO_A_Roles> roleManager)
    {
        _context = context;
        _logger = logger;
        _roleManager = roleManager;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        try
        {
            var roles = await _context.Roles.ToListAsync();
            if (!roles.Any())
                TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.InfoAlert("No hay roles registrados en el sistema"));
            return View(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de roles");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar la lista de roles"));
            return RedirectToAction("Index", "Home");
        }
    }

    public async Task<IActionResult> Search(string searchElement, string activeFilter)
    {
        try
        {
            ViewBag.SearchString = searchElement;
            ViewBag.ActiveFilter = activeFilter;

            var query = _roleManager.Roles;
            // Aplicar filtro de búsqueda si existe
            if (!string.IsNullOrEmpty(searchElement))
                query = query.Where(r => r.Name.Contains(searchElement));

            // Aplicar filtro de estado si no es "all"
            if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
            {
                var isActive = activeFilter == "true";
                query = query.Where(r => r.TB_Activo == isActive);
            }

            var roles = await query.ToListAsync();
            if (!roles.Any())
                TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.InfoAlert("No se encontraron roles con los filtros aplicados"));
            return View("Index", roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar roles");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al buscar roles"));
            return RedirectToAction("Index");
        }
    }

    public ViewResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name", "TB_Activo")] TECO_A_Roles rol)
    {
        if (!ModelState.IsValid)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Por favor, revise los datos ingresados"));
            return View(rol);
        }

        try
        {
            await _roleManager.CreateAsync(rol);
            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.SuccessAlert());
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al crear el rol"));
            return View(rol);
        }
    }

    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var rol = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (rol == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el rol"));
                return NotFound();
            }

            return View(rol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar rol para editar");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar el rol para editar"));
            return RedirectToAction("Index");
        }
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] TECO_A_Roles rol)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos enviados al servidor." });

            var existingRol = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == rol.Id);
            if (existingRol == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el rol"));
                return NotFound();
            }

            existingRol.Name = rol.Name;
            existingRol.TB_Activo = rol.TB_Activo;
            await _context.SaveChangesAsync();

            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.SuccessAlert());
            return Ok();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error al actualizar rol");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("No se pudo guardar los cambios en la base de datos"));
            return StatusCode(500, new { message = "No se pudo guardar los cambios en la base de datos" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(string id)
    {
        try
        {
            var rol = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (rol == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el rol"));
                return NotFound();
            }

            rol.TB_Activo = !rol.TB_Activo;
            _context.Roles.Update(rol);
            await _context.SaveChangesAsync();

            TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.InfoAlert($"Estado del rol {rol.Name} cambiado correctamente"));
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del rol");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cambiar el estado del rol"));
            return RedirectToAction("Index");
        }
    }
}