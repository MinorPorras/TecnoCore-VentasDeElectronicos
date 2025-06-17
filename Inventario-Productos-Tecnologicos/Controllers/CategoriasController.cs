using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class CategoriasController : Controller
{
    private readonly TecnoCoreDbContext _context;

    public CategoriasController(TecnoCoreDbContext context)
    {
        _context = context;
    }

    // GET: Categorias
    public async Task<IActionResult> Index()
    {
        var categorias = await _context.Categorias.ToListAsync();
        return View(categorias);
    }

    // GET: Categorias/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categorias/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Activo")] Categorias categoria)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(categoria);
    }

    public async Task<IActionResult> Search(string searchElement, string activeFilter)
    {
        ViewBag.SearchString = searchElement;
        ViewBag.ActiveFilter = activeFilter;

        var query = _context.Categorias.AsQueryable();

        // Aplicar filtro de búsqueda si existe
        if (!string.IsNullOrEmpty(searchElement))
            query = query.Where(c => c.Nombre.Contains(searchElement)
                                     || c.Id.ToString().Contains(searchElement));

        // Aplicar filtro de estado si no es "all"
        if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
        {
            var isActive = activeFilter == "true";
            query = query.Where(c => c.Activo == isActive);
        }

        var categorias = await query.ToListAsync();
        return View("Index", categorias);
    }

    // GET: Categorias/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        Console.WriteLine("Pasa por aquí edit");

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();
        ViewBag.Subcategorias = _context.Subcategorias.Where(s => s.CategoriaId == id).ToList();
        return View(categoria);
    }

    // POST: Categorias/Edit/5
    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Activo")] Categorias categoria)
    {
        Console.WriteLine("Pasa por aquí Edit submit");

        if (id != categoria.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(categoria.Id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(categoria);
    }

    // POST: Categorias/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Console.WriteLine("Pasa por aquí delete");

        var categoria = await _context.Categorias.FindAsync(id);
        var subcategorias = await _context.Subcategorias.Where(s => s.CategoriaId == id).ToListAsync();
        if (categoria != null)
        {
            foreach (var sub in subcategorias)
            {
                _context.Subcategorias.Remove(sub);
            }
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CategoriaExists(int id)
    {
        return _context.Categorias.Any(e => e.Id == id);
    }

    public IActionResult CreateSubcategoria(int idCategoria)
    {
        ViewBag.CategoriaId = idCategoria;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateSubcategoria([Bind("Nombre", "CategoriaId", "Activo")] Subcategorias subcategoria)
    {
        Console.WriteLine("Pasa acá");
        Console.WriteLine(subcategoria.Nombre);
        Console.WriteLine(subcategoria.CategoriaId);
        Console.WriteLine(subcategoria.Activo);
        if (ModelState.IsValid)
        {
            _context.Subcategorias.Add(subcategoria);
            _context.SaveChanges();
        }

        return RedirectToAction("Edit", new { id = subcategoria.CategoriaId });
    }

    public async Task<IActionResult> EditSubcategoria(int id)
    {
        var subcategoria = await _context.Subcategorias.FindAsync(id);
        return View(subcategoria);
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSubcategoria([FromBody] Subcategorias sub)
    {
        try
        {
            Console.WriteLine(ModelState.IsValid);
            if (!ModelState.IsValid) return BadRequest(new { message = "Datos inválidos enviados al servidor." });
            var existingSub = await _context.Subcategorias.FirstOrDefaultAsync(s => s.Id == sub.Id);
            if (existingSub == null) return NotFound();

            Console.WriteLine(sub.Nombre);
            Console.WriteLine(sub.CategoriaId);
            Console.WriteLine(sub.Activo);

            existingSub.Nombre = sub.Nombre;
            existingSub.Activo = sub.Activo;
            existingSub.CategoriaId = sub.CategoriaId; // Aseguramos que se mantenga la relación
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new { message = "No se pudo guardar los cambios en la base de datos" });
        }
    }

    // POST: Categorias/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSubcategoria(int id)
    {
        Console.WriteLine("Pasa por aquí");
        var subcategoria = await _context.Subcategorias.FindAsync(id);
        if (subcategoria != null)
        {
            _context.Subcategorias.Remove(subcategoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = subcategoria.CategoriaId });
        }

        return NotFound();
    }
}