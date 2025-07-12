using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
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
        var categorias = await _context.TECO_M_Categoria.ToListAsync();
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
    public async Task<IActionResult> Create([Bind("TC_Nombre,TB_Activo")] TECO_M_Categoria categoria)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categoria);
            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return RedirectToAction(nameof(Index));
        }

        TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
            Alert.ErrorAlert("Los datos ingresados no son válidos"));
        return View(categoria);
    }

    public async Task<IActionResult> Search(string searchElement, string activeFilter)
    {
        ViewBag.SearchString = searchElement;
        ViewBag.ActiveFilter = activeFilter;

        try
        {
            var query = _context.TECO_M_Categoria.AsQueryable();

            // Aplicar filtro de búsqueda si existe
            if (!string.IsNullOrEmpty(searchElement))
                query = query.Where(c => c.TC_Nombre.Contains(searchElement)
                                         || c.TN_Id.ToString().Contains(searchElement));

            // Aplicar filtro de estado si no es "all"
            if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
            {
                var isActive = activeFilter == "true";
                query = query.Where(c => c.TB_Activo == isActive);
            }

            var categorias = await query.ToListAsync();
            if (!categorias.Any())
                TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.InfoAlert("No se encontraron categorías con los criterios especificados"));
            return View("Index", categorias);
        }
        catch (Exception e)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al buscar categorías"));
            return NotFound();
        }
    }

    // GET: Categorias/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await _context.TECO_M_Categoria.FindAsync(id);
        if (categoria == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("la categoría"));
            return NotFound();
        }

        ViewBag.Subcategorias = _context.TECO_M_Subcategoria.Where(s => s.TN_CategoriaId == id).ToList();
        return View(categoria);
    }

    // POST: Categorias/Edit/5
    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] [Bind("TN_Id,TC_Nombre,TB_Activo")] TECO_M_Categoria categoria)
    {
        var existingCategory = await _context.TECO_M_Categoria.FindAsync(categoria.TN_Id);
        if (existingCategory == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("la categoría"));
            return NoContent();
        }

        existingCategory.TC_Nombre = categoria.TC_Nombre;
        existingCategory.TB_Activo = categoria.TB_Activo;

        var subcategorias = await _context.TECO_M_Subcategoria.Where(s => s.TN_CategoriaId == categoria.TN_Id)
            .ToListAsync();
        if (!categoria.TB_Activo)
            foreach (var sub in subcategorias)
            {
                sub.TB_Activo = false; // Cambia el estado de cada subcategoría
                _context.TECO_M_Subcategoria.Update(sub);
            }

        if (!ModelState.IsValid)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Los datos ingresados no son válidos"));
            return View(categoria);
        }

        try
        {
            _context.Update(existingCategory);
            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoriaExists(existingCategory.TN_Id))
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("la categoría"));
                return NotFound();
            }

            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error de concurrencia al actualizar la categoría"));
            return StatusCode(500);
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al actualizar la categoría"));
            return StatusCode(500);
        }
    }

    // POST: Categorias/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var categoria = await _context.TECO_M_Categoria.FindAsync(id);
        if (categoria == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("la categoría"));
            return RedirectToAction(nameof(Index));
        }

        var subcategorias = await _context.TECO_M_Subcategoria.Where(s => s.TN_CategoriaId == id).ToListAsync();

        if (categoria.TB_Activo)
            foreach (var sub in subcategorias)
            {
                sub.TB_Activo = false;
                _context.TECO_M_Subcategoria.Update(sub);
            }

        categoria.TB_Activo = !categoria.TB_Activo;
        _context.TECO_M_Categoria.Update(categoria);
        await _context.SaveChangesAsync();

        TempData["info"] =
            System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("Estado actualizado correctamente"));
        return RedirectToAction(nameof(Index));
    }

    private bool CategoriaExists(int id)
    {
        return _context.TECO_M_Categoria.Any(e => e.TN_Id == id);
    }

    public IActionResult CreateSubcategoria(int idCategoria)
    {
        if (idCategoria == 0)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Categoría no válida"));
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CategoriaId = idCategoria;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateSubcategoria(
        [Bind("TC_Nombre", "TN_CategoriaId", "TB_Activo")]
        TECO_M_Subcategoria subcategoria)
    {
        if (string.IsNullOrEmpty(subcategoria.TC_Nombre))
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("El nombre no puede estar vacío"));
            return View(subcategoria);
        }

        if (!ModelState.IsValid)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Los datos ingresados no son válidos"));
            return View(subcategoria);
        }

        _context.TECO_M_Subcategoria.Add(subcategoria);
        _context.SaveChanges();
        TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
        return RedirectToAction("Edit", new { Id = subcategoria.TN_CategoriaId });
    }

    public async Task<IActionResult> EditSubcategoria(int id)
    {
        var subcategoria = await _context.TECO_M_Subcategoria.FindAsync(id);
        if (subcategoria == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("la subcategoría"));
            return RedirectToAction(nameof(Index));
        }

        return View(subcategoria);
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSubcategoria([FromBody] TECO_M_Subcategoria sub)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Los datos ingresados no son válidos"));
                return BadRequest();
            }

            var existingSub = await _context.TECO_M_Subcategoria.FirstOrDefaultAsync(s => s.TN_Id == sub.TN_Id);
            if (existingSub == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("la subcategoría"));
                return NotFound();
            }

            var categoria = await _context.TECO_M_Categoria.Include(c => c.Subcategoria)
                .FirstOrDefaultAsync(c => c.Subcategoria.Any(s => s.TN_Id == existingSub.TN_Id));
            if (!existingSub.TB_Activo && categoria != null)
            {
                categoria.TB_Activo = true;
                _context.TECO_M_Categoria.Update(categoria);
            }

            existingSub.TC_Nombre = sub.TC_Nombre;
            existingSub.TB_Activo = sub.TB_Activo;
            existingSub.TN_CategoriaId = sub.TN_CategoriaId;
            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return Ok();
        }
        catch (DbUpdateException)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al guardar los cambios en la base de datos"));
            return StatusCode(500);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubCatSwitchActive(int id)
    {
        var subcategoria = await _context.TECO_M_Subcategoria.FindAsync(id);
        if (subcategoria == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("la subcategoría"));
            return RedirectToAction(nameof(Index));
        }

        var categoria = await _context.TECO_M_Categoria.Include(c => c.Subcategoria)
            .FirstOrDefaultAsync(c => c.Subcategoria.Any(s => s.TN_Id == subcategoria.TN_Id));

        if (!subcategoria.TB_Activo && categoria != null)
        {
            categoria.TB_Activo = true;
            _context.TECO_M_Categoria.Update(categoria);
        }

        subcategoria.TB_Activo = !subcategoria.TB_Activo;
        _context.TECO_M_Subcategoria.Update(subcategoria);
        await _context.SaveChangesAsync();

        TempData["info"] =
            System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("Estado actualizado correctamente"));
        return RedirectToAction("Edit", new { Id = subcategoria.TN_CategoriaId });
    }
}