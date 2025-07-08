using System.Runtime.InteropServices.JavaScript;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Inventario_Productos_Tecnologicos.Controllers;

public class KardexController : Controller
{
    private readonly TecnoCoreDbContext _context;

    public KardexController(TecnoCoreDbContext context)
    {
        _context = context;
    }

    // GET: Kardex
// En KardexController.cs
    public async Task<IActionResult> Index()
    {
        var kardexes = await _context.Kardex
            .Include(k => k.Producto)
            .Include(k => k.TipoMovimientoKardex)
            .ToListAsync();

        // Enviar la lista de tipos de movimiento directamente
        ViewBag.TiposMovimiento = await _context.TipoMovimientoKardex
            .Where(t => t.Activo == true)
            .ToListAsync();

        return View(kardexes);
    }

    // GET: Kardex/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var kardex = await _context.Kardex
            .Include(k => k.Producto)
            .Include(k => k.TipoMovimientoKardex)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (kardex == null) return NotFound();

        return View(kardex);
    }

    public async Task<IActionResult> CreateEntry()
    {
        var viewModel = new Models.ViewModels.KardexViewModel
        {
            Fecha = DateTime.Now,
            ProductosDisponibles = await _context.Productos
                .Where(p => p.Activo == true)
                .ToListAsync()
        };

        ViewData["TipoMovimientoId"] =
            new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.TipoMovimientoKardex.Where(t => t.Activo == true && t.Entrada).ToListAsync(),
                "Id", "Tipo");

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEntry(Models.ViewModels.KardexViewModel viewModel)
    {
        if (ModelState.IsValid)
            try
            {
                var kardex = new Kardex
                {
                    ProductoId = viewModel.ProductoId,
                    Fecha = viewModel.Fecha,
                    TipoMovimientoId = viewModel.TipoMovimientoId,
                    Cantidad = viewModel.Cantidad,
                    Descripcion = viewModel.Descripcion,
                    Activo = viewModel.Activo
                };

                if (kardex.ProductoId.HasValue)
                {
                    var producto = await _context.Productos.FindAsync(kardex.ProductoId);
                    if (producto != null)
                    {
                        kardex.StockAnterior = producto.Stock;
                        var tipoMovimiento = await _context.TipoMovimientoKardex.FindAsync(kardex.TipoMovimientoId);
                        if (tipoMovimiento != null && tipoMovimiento.Entrada)
                        {
                            producto.Stock += kardex.Cantidad ?? 0;
                            kardex.StockActual = producto.Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el movimiento de kardex: " + ex.Message);
            }

        viewModel.ProductosDisponibles = await _context.Productos
            .Where(p => p.Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.TipoMovimientoKardex.Where(t => t.Activo == true && t.Entrada).ToListAsync(),
            "Id", "Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    public async Task<IActionResult> CreateExit()
    {
        var viewModel = new Models.ViewModels.KardexViewModel
        {
            Fecha = DateTime.Now,
            ProductosDisponibles = await _context.Productos
                .Where(p => p.Activo == true)
                .ToListAsync()
        };

        ViewData["TipoMovimientoId"] =
            new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.TipoMovimientoKardex.Where(t => t.Activo == true && !t.Entrada).ToListAsync(),
                "Id", "Tipo");

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExit(Models.ViewModels.KardexViewModel viewModel)
    {
        if (ModelState.IsValid)
            try
            {
                var kardex = new Kardex
                {
                    ProductoId = viewModel.ProductoId,
                    Fecha = viewModel.Fecha,
                    TipoMovimientoId = viewModel.TipoMovimientoId,
                    Cantidad = viewModel.Cantidad,
                    Descripcion = viewModel.Descripcion,
                    Activo = viewModel.Activo
                };

                if (kardex.ProductoId.HasValue)
                {
                    var producto = await _context.Productos.FindAsync(kardex.ProductoId);
                    if (producto != null)
                    {
                        kardex.StockAnterior = producto.Stock;
                        var tipoMovimiento = await _context.TipoMovimientoKardex.FindAsync(kardex.TipoMovimientoId);
                        if (tipoMovimiento != null && !tipoMovimiento.Entrada)
                        {
                            producto.Stock += kardex.Cantidad ?? 0;
                            kardex.StockActual = producto.Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el movimiento de kardex: " + ex.Message);
            }

        viewModel.ProductosDisponibles = await _context.Productos
            .Where(p => p.Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.TipoMovimientoKardex.Where(t => t.Activo == true && !t.Entrada).ToListAsync(),
            "Id", "Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductoStock(int id)
    {
        var stock = await _context.Productos
            .Where(p => p.Id == id)
            .Select(p => new { stock = p.Stock })
            .FirstOrDefaultAsync();

        if (stock == null) return NotFound();
        return Json(stock);
    }

    [HttpGet]
    public async Task<IActionResult> GetTipoMovimiento(int id)
    {
        var tipoMovimiento = await _context.TipoMovimientoKardex
            .Where(t => t.Id == id)
            .Select(t => new { esEntrada = t.Entrada })
            .FirstOrDefaultAsync();

        if (tipoMovimiento == null) return NotFound();
        return Json(tipoMovimiento);
    }

    // POST: Kardex/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var kardex = await _context.Kardex.FindAsync(id);
        if (kardex != null)
        {
            kardex.Activo = !kardex.Activo;
            _context.Kardex.Update(kardex);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Search(string searchElement, string searchDate, string activeFilter = "all",
        string tipoMovimiento = "all")
    {
        // Guardar los parámetros de búsqueda en ViewBag
        ViewBag.SearchString = searchElement;
        ViewBag.SearchDate = searchDate;
        ViewBag.ActiveFilter = activeFilter;
        ViewBag.SelectedTipoMovimiento = tipoMovimiento;

        // Crear la consulta base
        var query = _context.Kardex
            .Include(k => k.Producto)
            .Include(k => k.TipoMovimientoKardex)
            .AsQueryable();

        // Filtrar por nombre de producto o descripción
        if (!string.IsNullOrEmpty(searchElement))
            query = query.Where(k =>
                (k.Producto != null && k.Producto.Nombre.Contains(searchElement)) ||
                (k.Descripcion != null && k.Descripcion.Contains(searchElement)));

        // Filtrar por fecha
        if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParse(searchDate, out var date))
            query = query.Where(k => k.Fecha.HasValue && k.Fecha.Value.Date == date.Date);

        // Filtrar por estado activo/inactivo
        if (activeFilter != "all")
        {
            var isActive = activeFilter == "true";
            query = query.Where(k => k.Activo == isActive);
        }

        // Filtrar por tipo de movimiento
        if (tipoMovimiento != "all" && int.TryParse(tipoMovimiento, out var tipoMovimientoId))
            query = query.Where(k => k.TipoMovimientoId == tipoMovimientoId);

        // Cargar los tipos de movimiento para el dropdown
        ViewBag.TiposMovimiento = await _context.TipoMovimientoKardex
            .Where(t => t.Activo == true)
            .ToListAsync();

        // Ejecutar la consulta
        var kardexes = await query.ToListAsync();
        return View("Index", kardexes);
    }
}