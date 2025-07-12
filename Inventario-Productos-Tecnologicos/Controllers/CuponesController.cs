using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inventario_Productos_Tecnologicos.Models.ViewModels;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class CuponesController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CuponesController> _logger;

    private readonly Dictionary<string, string> _tipoDescuento = new()
    {
        { "P", "Porcentaje" },
        { "M", "Monto" }
    };

    public CuponesController(TecnoCoreDbContext context, ILogger<CuponesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var cupones = await _context.TECO_M_Cupon.ToListAsync();

            _logger.LogInformation("Cupones cargados: {Count}", cupones.Count);
            foreach (var cupon in cupones)
                _logger.LogInformation("Cupón ID: {Id}, Código: {Codigo}", cupon.TN_Id, cupon.TC_Codigo);

            ViewData["TipoDescuento"] = _tipoDescuento;
            return View(cupones);
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar la lista de cupones"));
            _logger.LogError(ex, "Error al cargar cupones");
            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult Create()
    {
        ViewBag.TipoDescuento = new SelectList(_tipoDescuento, "Key", "Value");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(
            "TC_Codigo,TC_Descripcion,TC_TipoDescuento,TN_Valor,TF_FechaInicio,TF_FechaFin,TN_UsosActuales,TN_UsosMaximos,TB_Activo")]
        TECO_M_Cupon cupon)
    {
        if (ModelState.IsValid)
            try
            {
                _context.Add(cupon);
                await _context.SaveChangesAsync();
                TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Error al crear el cupón"));
                _logger.LogError(ex, "Error al crear cupón");
            }

        ViewBag.TipoDescuento = new SelectList(_tipoDescuento, "Key", "Value", cupon.TC_TipoDescuento);
        TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
            Alert.ErrorAlert("Por favor, revise los datos ingresados"));
        return View(cupon);
    }

    public async Task<IActionResult> Edit(int TN_Id)
    {
        Console.WriteLine($"Edit method called with id: {TN_Id}");
        var cupon = await _context.TECO_M_Cupon.FindAsync(TN_Id);
        if (cupon == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("el cupón"));
            return NotFound();
        }

        ViewBag.TipoDescuento = new SelectList(_tipoDescuento, "Key", "Value", cupon.TC_TipoDescuento);
        return View(cupon);
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] TECO_M_Cupon cupon)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Los datos ingresados no son válidos"));
                return BadRequest();
            }

            var cuponExistente = await _context.TECO_M_Cupon.FindAsync(cupon.TN_Id);
            if (cuponExistente == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el cupón"));
                return NotFound();
            }

            // Actualizar las propiedades del cupón existente
            cuponExistente.TC_Codigo = cupon.TC_Codigo;
            cuponExistente.TC_Descripcion = cupon.TC_Descripcion;
            cuponExistente.TC_TipoDescuento = cupon.TC_TipoDescuento;
            cuponExistente.TN_Valor = cupon.TN_Valor;
            cuponExistente.TF_FechaInicio = cupon.TF_FechaInicio;
            cuponExistente.TF_FechaFin = cupon.TF_FechaFin;
            cuponExistente.TN_UsosActuales = cupon.TN_UsosActuales;
            cuponExistente.TN_UsosMaximos = cupon.TN_UsosMaximos;
            cuponExistente.TB_Activo = cupon.TB_Activo;

            _context.Update(cuponExistente);
            await _context.SaveChangesAsync();

            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CuponExists(cupon.TN_Id))
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el cupón"));
                return NotFound();
            }

            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error de concurrencia al actualizar el cupón"));
            return StatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cupón {CuponId}", cupon.TN_Id);
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al actualizar el cupón"));
            return StatusCode(500);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        try
        {
            var cupon = await _context.TECO_M_Cupon.FindAsync(id);
            if (cupon == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el cupón"));
                return RedirectToAction(nameof(Index));
            }

            cupon.TB_Activo = !cupon.TB_Activo;
            _context.Update(cupon);
            await _context.SaveChangesAsync();

            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cambiar el estado del cupón"));
            _logger.LogError(ex, "Error al cambiar estado del cupón {CuponId}", id);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CuponExists(int id)
    {
        return _context.TECO_M_Cupon.Any(e => e.TN_Id == id);
    }

    public async Task<IActionResult> Search(string searchElement, string activeFilter)
    {
        try
        {
            ViewBag.SearchString = searchElement;
            ViewBag.ActiveFilter = activeFilter;

            var query = _context.TECO_M_Cupon.AsQueryable();

            if (!string.IsNullOrEmpty(searchElement))
                query = query.Where(c => c.TC_Codigo.Contains(searchElement)
                                         || c.TC_Descripcion.Contains(searchElement));

            if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
            {
                var isActive = activeFilter == "true";
                query = query.Where(c => c.TB_Activo == isActive);
            }

            var cupones = await query.ToListAsync();

            if (!cupones.Any())
                TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.InfoAlert("No se encontraron cupones con los criterios especificados"));

            ViewData["TipoDescuento"] = _tipoDescuento;
            return View("Index", cupones);
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al buscar cupones"));
            _logger.LogError(ex, "Error al buscar cupones");
            return RedirectToAction(nameof(Index));
        }
    }
}