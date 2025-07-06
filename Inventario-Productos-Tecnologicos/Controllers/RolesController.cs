/*using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class RolesController : Controller
{
    private readonly TecnoCoreDbContext _context;

    public RolesController(TecnoCoreDbContext context)
    {
        _context = context;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View(await _context.Roles.ToListAsync());
    }

    public async Task<IActionResult> Search(string searchElement, string activeFilter)
    {
        ViewBag.SearchString = searchElement;
        ViewBag.ActiveFilter = activeFilter;

        var query = _context.Roles.AsQueryable();

        // Aplicar filtro de búsqueda si existe
        if (!string.IsNullOrEmpty(searchElement))
            query = query.Where(r => r.Name.Contains(searchElement)
                                     || r.Id.ToString().Contains(searchElement));

        // Aplicar filtro de estado si no es "all"
        if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
        {
            var isActive = activeFilter == "true";
            query = query.Where(r => r.Activo == isActive);
        }

        var roles = await query.ToListAsync();
        return View("Index", roles);
    }

    public ViewResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name", "Activo")] Roles rol)
    {
        if (!ModelState.IsValid) return View(rol);
        try
        {
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return View(rol);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null) return NotFound();
        return View(rol);
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] Roles rol)
    {
        Console.WriteLine("llega aquí");
        try
        {
            Console.WriteLine(ModelState.IsValid);
            if (!ModelState.IsValid) return BadRequest(new { message = "Datos inválidos enviados al servidor." });
            var existingRol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == rol.Id);
            if (existingRol == null) return NotFound();
            Console.WriteLine(rol.Name, rol.Activo);
            existingRol.Name = rol.Name;
            existingRol.Activo = rol.Activo;
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new { message = "No se pudo guardar los cambios en la base de datos" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null) return NotFound();
        rol.Activo = !rol.Activo; // Cambia el estado de activo a inactivo o viceversa
        _context.Roles.Update(rol);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}*/

