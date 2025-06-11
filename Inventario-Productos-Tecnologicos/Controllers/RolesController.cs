using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class RolesController : Controller
{
    private readonly TecnoCoreDbContext _context = new();


    // GET
    public async Task<IActionResult> Index()
    {
        var roles = await _context.Roles.ToListAsync();
        return View(roles);
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
            if (!ModelState.IsValid) return BadRequest(new { message = "Datos inválidos enviados al servidor." });
            var existingRol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == rol.Id);
            if (existingRol == null) return NotFound();
            existingRol.Name = rol.Name;
            existingRol.Activo = rol.Activo;
            _context.Roles.Update(existingRol);
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
    public async Task<IActionResult> Delete(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null) return NotFound();
        _context.Roles.Remove(rol);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}