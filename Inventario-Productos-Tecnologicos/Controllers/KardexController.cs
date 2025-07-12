using System.Runtime.InteropServices.JavaScript;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Models.ViewModels;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class KardexController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<KardexController> _logger;

    public KardexController(TecnoCoreDbContext context, ILogger<KardexController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Kardex
    public async Task<IActionResult> Index()
    {
        try
        {
            var kardexes = await _context.TECO_P_Kardex
                .Include(k => k.Producto)
                .Include(k => k.TipoMovimientoKardex)
                .ToListAsync();

            ViewBag.TiposMovimiento = await _context.TECO_M_TipoMovimientoKardex
                .Where(t => t.TB_Activo == true)
                .ToListAsync();

            return View(kardexes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de kardex");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar la lista de kardex"));
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: Kardex/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var kardex = await _context.TECO_P_Kardex
                .Include(k => k.Producto)
                .Include(k => k.TipoMovimientoKardex)
                .FirstOrDefaultAsync(m => m.TN_Id == id);

            if (kardex == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el movimiento de kardex"));
                return RedirectToAction(nameof(Index));
            }

            return View(kardex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar los detalles del kardex {KardexId}", id);
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar los detalles del movimiento"));
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> CreateEntry()
    {
        try
        {
            var viewModel = new KardexViewModel
            {
                Fecha = DateTime.Now,
                ProductosDisponibles = await _context.TECO_A_Producto
                    .Where(p => p.TB_Activo == true)
                    .ToListAsync()
            };

            ViewData["TipoMovimientoId"] =
                new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await _context.TECO_M_TipoMovimientoKardex.Where(t => t.TB_Activo == true && t.TB_Entrada)
                        .ToListAsync(),
                    "TN_Id", "TC_Tipo");

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el formulario de entrada de kardex");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar el formulario"));
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEntry(KardexViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var kardex = new TECO_P_Kardex
                {
                    TN_ProductoId = viewModel.ProductoId,
                    TF_Fecha = viewModel.Fecha,
                    TN_TipoMovimientoId = viewModel.TipoMovimientoId,
                    TN_Cantidad = viewModel.Cantidad,
                    TC_Descripcion = viewModel.Descripcion,
                    TB_Activo = viewModel.Activo
                };

                if (kardex.TN_ProductoId.HasValue)
                {
                    var producto = await _context.TECO_A_Producto.FindAsync(kardex.TN_ProductoId);
                    if (producto != null)
                    {
                        kardex.TN_StockAnterior = producto.TN_Stock;
                        var tipoMovimiento =
                            await _context.TECO_M_TipoMovimientoKardex.FindAsync(kardex.TN_TipoMovimientoId);
                        if (tipoMovimiento != null && tipoMovimiento.TB_Entrada)
                        {
                            producto.TN_Stock += kardex.TN_Cantidad ?? 0;
                            kardex.TN_StockActual = producto.TN_Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
                return RedirectToAction(nameof(Index));
            }

            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Por favor, revise los datos ingresados"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear entrada de kardex");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al registrar la entrada"));
        }

        viewModel.ProductosDisponibles = await _context.TECO_A_Producto
            .Where(p => p.TB_Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.TECO_M_TipoMovimientoKardex.Where(t => t.TB_Activo == true && t.TB_Entrada).ToListAsync(),
            "TN_Id", "TC_Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    public async Task<IActionResult> CreateExit()
    {
        try
        {
            var viewModel = new KardexViewModel
            {
                Fecha = DateTime.Now,
                ProductosDisponibles = await _context.TECO_A_Producto
                    .Where(p => p.TB_Activo == true)
                    .ToListAsync()
            };

            ViewData["TipoMovimientoId"] =
                new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await _context.TECO_M_TipoMovimientoKardex
                        .Where(t => t.TB_Activo == true && t.TB_Entrada == false)
                        .ToListAsync(),
                    "TN_Id", "TC_Tipo");

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el formulario de salida de kardex");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar el formulario"));
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExit(KardexViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var kardex = new TECO_P_Kardex
                {
                    TN_ProductoId = viewModel.ProductoId,
                    TF_Fecha = viewModel.Fecha,
                    TN_TipoMovimientoId = viewModel.TipoMovimientoId,
                    TN_Cantidad = viewModel.Cantidad,
                    TC_Descripcion = viewModel.Descripcion,
                    TB_Activo = viewModel.Activo
                };

                if (kardex.TN_ProductoId.HasValue)
                {
                    var producto = await _context.TECO_A_Producto.FindAsync(kardex.TN_ProductoId);
                    if (producto != null)
                    {
                        if (producto.TN_Stock < (kardex.TN_Cantidad ?? 0))
                        {
                            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                                Alert.ErrorAlert("No hay suficiente stock disponible"));
                            return RedirectToAction(nameof(CreateExit));
                        }

                        kardex.TN_StockAnterior = producto.TN_Stock;
                        var tipoMovimiento =
                            await _context.TECO_M_TipoMovimientoKardex.FindAsync(kardex.TN_TipoMovimientoId);
                        if (tipoMovimiento is { TB_Entrada: false })
                        {
                            producto.TN_Stock -= kardex.TN_Cantidad ?? 0;
                            kardex.TN_StockActual = producto.TN_Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
                return RedirectToAction(nameof(Index));
            }

            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Por favor, revise los datos ingresados"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear salida de kardex");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al registrar la salida"));
        }

        viewModel.ProductosDisponibles = await _context.TECO_A_Producto
            .Where(p => p.TB_Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.TECO_M_TipoMovimientoKardex.Where(t => t.TB_Activo == true && !t.TB_Entrada).ToListAsync(),
            "TN_Id", "TC_Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductoStock(int id)
    {
        var stock = await _context.TECO_A_Producto
            .Where(p => p.TN_Id == id)
            .Select(p => new { stock = p.TN_Stock })
            .FirstOrDefaultAsync();

        if (stock == null) return NotFound();
        return Json(stock);
    }

    [HttpGet]
    public async Task<IActionResult> GetTipoMovimiento(int id)
    {
        var tipoMovimiento = await _context.TECO_M_TipoMovimientoKardex
            .Where(t => t.TN_Id == id)
            .Select(t => new { esEntrada = t.TB_Entrada })
            .FirstOrDefaultAsync();

        if (tipoMovimiento == null) return NotFound();
        return Json(tipoMovimiento);
    }

    // POST: Kardex/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var kardex = await _context.TECO_P_Kardex.FindAsync(id);
        if (kardex != null)
        {
            kardex.TB_Activo = !kardex.TB_Activo;
            _context.TECO_P_Kardex.Update(kardex);
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
        var query = _context.TECO_P_Kardex
            .Include(k => k.Producto)
            .Include(k => k.TipoMovimientoKardex)
            .AsQueryable();

        // Filtrar por nombre de producto o descripción
        if (!string.IsNullOrEmpty(searchElement))
            query = query.Where(k =>
                (k.Producto != null && k.Producto.TC_Nombre.Contains(searchElement)) ||
                (k.TC_Descripcion != null && k.TC_Descripcion.Contains(searchElement)));

        // Filtrar por fecha
        if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParse(searchDate, out var date))
            query = query.Where(k => k.TF_Fecha.HasValue && k.TF_Fecha.Value.Date == date.Date);

        // Filtrar por estado activo/inactivo
        if (activeFilter != "all")
        {
            var isActive = activeFilter == "true";
            query = query.Where(k => k.TB_Activo == isActive);
        }

        // Filtrar por tipo de movimiento
        if (tipoMovimiento != "all" && int.TryParse(tipoMovimiento, out var tipoMovimientoId))
            query = query.Where(k => k.TN_TipoMovimientoId == tipoMovimientoId);

        // Cargar los tipos de movimiento para el dropdown
        ViewBag.TiposMovimiento = await _context.TECO_M_TipoMovimientoKardex
            .Where(t => t.TB_Activo == true)
            .ToListAsync();

        // Ejecutar la consulta
        var kardexes = await query.ToListAsync();
        return View("Index", kardexes);
    }
}