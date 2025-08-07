using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Text.Json.JsonSerializer;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class CategoriasController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(TecnoCoreDbContext context, ILogger<CategoriasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Categorias
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Index()
    {
        var categorias = await _context.TECO_M_Categoria.ToListAsync();
        return View(categorias);
    }

    // GET: Categorias/Create
    [Authorize(Roles = "Administrador")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categorias/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([Bind("TC_Nombre,TB_Activo")] TECO_M_Categoria categoria)
    {
        if (ModelState.IsValid)
        {
            var existingCategory = await _context.TECO_M_Categoria.Where(c => c.TC_Nombre == categoria.TC_Nombre).FirstOrDefaultAsync();
            if (existingCategory != null)
            {
                ViewBag.Alert = Alert.ErrorAlert("Ya existe una categoría con ese nombre");
                return View(categoria);
            }
            _context.Add(categoria);
            await _context.SaveChangesAsync();
            TempData["success"] = Serialize(Alert.SuccessAlert());
            return RedirectToAction(nameof(Index));
        }

        TempData["Alert"] = Serialize(
            Alert.ErrorAlert("Los datos ingresados no son válidos"));
        return View(categoria);
    }

    [Authorize(Roles = "Administrador")]
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
                TempData["info"] = Serialize(
                    Alert.InfoAlert("No se encontraron categorías con los criterios especificados"));
            return View("Index", categorias);
        }
        catch (Exception e)
        {
            TempData["Alert"] = Serialize(
                Alert.ErrorAlert($"Error al buscar categorías: {e.Message}"));
            return NotFound();
        }
    }

    // GET: Categorias/Edit/5
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await _context.TECO_M_Categoria.FindAsync(id);
        if (categoria == null)
        {
            TempData["Alert"] = Serialize(
                Alert.NotFoundAlert("la categoría"));
            return NotFound();
        }

        ViewBag.Subcategorias = _context.TECO_M_Subcategoria.Where(s => s.TN_CategoriaId == id).ToList();
        return View(categoria);
    }

    // POST: Categorias/Edit/5
    [HttpPut]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit([FromBody] [Bind("TN_Id,TC_Nombre,TB_Activo")] TECO_M_Categoria categoria)
    {
        var existingCategory = await _context.TECO_M_Categoria.FindAsync(categoria.TN_Id);
        if (existingCategory == null)
        {
            ViewBag.Alert = Alert.NotFoundAlert("No se encontró la categoría a modificar");
            return BadRequest(new { success = false, message = "No se encontró la categoría a modificar"});
        }
        
        var existingCategoryName = await _context.TECO_M_Categoria.Where(c => c.TC_Nombre == categoria.TC_Nombre).FirstOrDefaultAsync();
        //Si se encontró alguna categoría con el mismo nombre
        if (existingCategoryName != null)
        {
            //Se verifica que no sea la misma categoría
            if (existingCategoryName.TN_Id != categoria.TN_Id)
            {
                //Se devuelve un error indicando que ya existe una categoría con ese nombre
                ViewBag.Alert = Alert.NotFoundAlert("Ya existe una categoría con ese nombre");
                return BadRequest(new { success = false, message = "Ya existe una categoría con ese nombre"});
            }
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
            ViewBag.Alert = Alert.ErrorAlert("Los datos ingresados no son válidos");
            return BadRequest(new { success = false, message = "Los datos ingresados no son válidos"});
        }

        try
        {
            _context.Update(existingCategory);
            await _context.SaveChangesAsync();
            TempData["success"] = Serialize(Alert.SuccessAlert());
            return Ok(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoriaExists(existingCategory.TN_Id))
            {
                ViewBag.Alert = Alert.NotFoundAlert("No se encontró la categoría a actualizar");
                return NotFound();
            }

            ViewBag.Alert = Alert.ErrorAlert("Error de concurrencia al actualizar la categoría");
            return StatusCode(500, new { success = false, message = $"Error interno del servidor: Concurrencia" });
        }
        catch (Exception ex)
        {
            ViewBag.Alert = Alert.ErrorAlert("Error al actualizar la categoría");
            return StatusCode(500, new { success = false, message = $"Error interno del servidor: {ex.Message}" });
        }
    }

    // POST: Categorias/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var categoria = await _context.TECO_M_Categoria.FindAsync(id);
        if (categoria == null)
        {
            TempData["Alert"] = Serialize(
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
            Serialize(Alert.InfoAlert("Estado actualizado correctamente"));
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    private bool CategoriaExists(int id)
    {
        return _context.TECO_M_Categoria.Any(e => e.TN_Id == id);
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult CreateSubcategoria(int idCategoria)
    {
        if (idCategoria == 0)
        {
            TempData["Alert"] = Serialize(
                Alert.ErrorAlert("Categoría no válida"));
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CategoriaId = idCategoria;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CreateSubcategoria(
        [Bind("TC_Nombre", "TN_CategoriaId", "TB_Activo")]
        TECO_M_Subcategoria subcategoria)
    {
        if (string.IsNullOrEmpty(subcategoria.TC_Nombre))
        {
            _logger.LogCritical("El nombre no puede estar vacío");
            ViewBag.Alert = Alert.ErrorAlert("El nombre no puede estar vacío");
            ViewBag.CategoriaId = subcategoria.TN_CategoriaId;
            return View(subcategoria);
        }

        if (!ModelState.IsValid)
        {
            _logger.LogCritical("Los datos ingresados no son válidos");
            _logger.LogCritical(Serialize(subcategoria));
            ViewBag.Alert = Alert.ErrorAlert("Los datos ingresados no son válidos");
            ViewBag.CategoriaId = subcategoria.TN_CategoriaId;
            return View(subcategoria);
        }
        
        var existingSub = await _context.TECO_M_Subcategoria.Where(s => s.TC_Nombre == subcategoria.TC_Nombre).FirstOrDefaultAsync();
        if (existingSub != null)
        {
            ViewBag.Alert = Alert.ErrorAlert("Ya existe una subcategoría con ese nombre");
            ViewBag.CategoriaId = subcategoria.TN_CategoriaId;
            return View(subcategoria);
        }

        _context.TECO_M_Subcategoria.Add(subcategoria);
        _context.SaveChanges();
        TempData["success"] = Serialize(Alert.SuccessAlert());
        return RedirectToAction("Edit", new { Id = subcategoria.TN_CategoriaId });
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EditSubcategoria(int idSubCategoria)
    {
        var subcategoria = await _context.TECO_M_Subcategoria.FindAsync(idSubCategoria);
        if (subcategoria != null) return View(subcategoria);
        TempData["Alert"] = Serialize(
            Alert.NotFoundAlert("la subcategoría"));
        return RedirectToAction(nameof(Edit));
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EditSubcategoria([FromBody] TECO_M_Subcategoria sub)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Alert = Alert.ErrorAlert("Los datos ingresados no son válidos");
                return BadRequest(new { success = false, message = "Los datos ingresados no son válidos"});
            }

            var existingSub = await _context.TECO_M_Subcategoria.FirstOrDefaultAsync(s => s.TN_Id == sub.TN_Id);
            if (existingSub == null)
            {
                ViewBag.Alert = Alert.NotFoundAlert("la subcategoría");
                return NotFound(new { success = false, message = "No se encontró la subcategoría a modificar"});
            }
            
            var existingSubName = await _context.TECO_M_Subcategoria.Where(s => s.TC_Nombre == sub.TC_Nombre).FirstOrDefaultAsync();
            if (existingSubName != null)
            {
                if (existingSubName.TN_Id != existingSub.TN_Id)
                {
                    ViewBag.Alert = Alert.ErrorAlert("Los datos ingresados no son válidos");
                    return BadRequest(new { success = false, message = "Ya hay una subcategoría con el mismo nombre"});
                }
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
            TempData["success"] = Serialize(Alert.SuccessAlert());
            return Ok(new { success = true, redirectUrl = Url.Action(nameof(Edit), new { id = sub.TN_CategoriaId }) });
        }
        catch (DbUpdateException)
        {
            TempData["Alert"] = Serialize(
                Alert.ErrorAlert("Error al guardar los cambios en la base de datos"));
            return StatusCode(500, new { success = false, message = $"Error interno del servidor: Error al guardar los cambios" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> SubCatSwitchActive(int id)
    {
        var subcategoria = await _context.TECO_M_Subcategoria.FindAsync(id);
        if (subcategoria == null)
        {
            TempData["Alert"] = Serialize(
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
            Serialize(Alert.InfoAlert("Estado actualizado correctamente"));
        return RedirectToAction("Edit", new { Id = subcategoria.TN_CategoriaId });
    }
}