using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Models.ViewModels;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class CuponesController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CuponesController> _logger;

    private readonly Dictionary<string, string> _tipoDescuento = new()
    {
        { "1", "Porcentaje" },
        { "2", "Fijo" }
    };
    //TODO No se está cargando el tipo de descuento en la vista Create, Edit y Index.

    public CuponesController(TecnoCoreDbContext context, ILogger<CuponesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var cupones = await _context.Cupones.ToListAsync();
        ViewData["TipoDescuento"] = _tipoDescuento;
        return View(cupones);
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
            "Codigo,Descripcion,TipoDescuento,Valor,FechaInicio,FechaFin, UsosActuales, UsosMaximos ,Activo")]
        Cupones cupon)
    {
        _logger.LogDebug(ModelState.IsValid.ToString());
        if (ModelState.IsValid)
        {
            _logger.LogInformation(cupon.Codigo);
            _context.Add(cupon);
            await _context.SaveChangesAsync();
            var alert = new Alert { Type = "success", Message = "Cupón creado exitosamente" };
            TempData["Sucess"] = System.Text.Json.JsonSerializer.Serialize(alert);
            return RedirectToAction(nameof(Index));
        }

        var errorAlert = new Alert { Type = "danger", Message = "Por favor, revise los datos ingresados" };
        TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(errorAlert);
        ViewBag.TipoDescuento = new SelectList(_tipoDescuento, "Key", "Value");
        return View(cupon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var cupon = await _context.Cupones.FindAsync(id);
        if (cupon == null) return RedirectToAction(nameof(Index));
        cupon.Activo = !cupon.Activo;
        _context.Cupones.Update(cupon);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cupon = await _context.Cupones.FindAsync(id);
        if (cupon == null) return NotFound();

        var tipoDescuentoKey = cupon.TipoDescuento;
        _logger.LogInformation($"---------------- {tipoDescuentoKey} -----------------");
        ViewBag.TipoDescuento = new SelectList(_tipoDescuento, "Key", "Value", tipoDescuentoKey);
        return View(cupon);
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] Cupones cupon)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Datos inválidos enviados al servidor.";
            return BadRequest(new
            {
                status = "error",
                message = "Datos inválidos enviados al servidor.",
                type = "danger"
            });
        }

        try
        {
            var existingCupon = await _context.Cupones.FirstOrDefaultAsync(r => r.Id == cupon.Id);
            if (existingCupon == null)
                return NotFound();

            // Verificar si el código ya existe en otro cupón
            var duplicateCupon = await _context.Cupones
                .FirstOrDefaultAsync(c => c.Codigo == cupon.Codigo && c.Id != cupon.Id);
            if (duplicateCupon != null)
                return BadRequest(new
                {
                    status = "error",
                    message = "Ya existe otro cupón con el mismo código.",
                    type = "danger"
                });

            // Validar valor según tipo de descuento
            if (cupon.TipoDescuento == "1" && (cupon.Valor < 0 || cupon.Valor > 100))
                return BadRequest(new
                {
                    status = "error",
                    message = "El porcentaje debe de estar entre 0 y 100.",
                    type = "danger"
                });
            if (cupon.TipoDescuento == "2" && cupon.Valor <= 0)
                return BadRequest(new
                {
                    status = "error",
                    message = "El valor del descuento fijo debe ser mayor que cero.",
                    type = "danger"
                });
            if (cupon.FechaInicio > cupon.FechaFin)
                return BadRequest(new
                {
                    status = "error",
                    message = "La fecha de inicio no puede ser posterior a la fecha de fin.",
                    type = "danger"
                });
            existingCupon.Codigo = cupon.Codigo;
            existingCupon.Descripcion = cupon.Descripcion;
            existingCupon.TipoDescuento = cupon.TipoDescuento;
            existingCupon.Valor = cupon.Valor;
            existingCupon.FechaInicio = cupon.FechaInicio;
            existingCupon.FechaFin = cupon.FechaFin;
            existingCupon.UsosActuales = cupon.UsosActuales;
            existingCupon.UsosMaximos = cupon.UsosMaximos;
            existingCupon.Activo = cupon.Activo;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cupón actualizado correctamente.";
            return Ok(new
            {
                status = "success",
                message = "Cupón actualizado correctamente.",
                type = "success"
            });
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new
            {
                status = "error",
                message = "No se pudo guardar los cambios en la base de datos",
                type = "danger"
            });
        }
    }

    public async Task<IActionResult> Search(string searchElement,
        string tipoDescuento = "all", string activeFilter = "all")
    {
        //Se guardan y envián los datos de la busqueda actual
        ViewBag.SearchString = searchElement;
        ViewBag.ActiveFilter = activeFilter;
        ViewBag.SelectedTipoDescuento = tipoDescuento;
        ViewBag.TipoDescuento = _tipoDescuento;

        try
        {
            var query = _context.Cupones.AsQueryable();
            if (!string.IsNullOrEmpty(searchElement))
                query = query.Where(r => r.Descripcion != null
                                         && (r.Codigo.Contains(searchElement)
                                             || r.Descripcion.Contains(searchElement)));

            // Filtrar por estado activo/inactivo
            if (activeFilter != "all")
            {
                var isActive = activeFilter == "true";
                query = query.Where(k => k.Activo == isActive);
            }

            //Filtrar por tipo de descuento
            if (tipoDescuento != "all") query = query.Where(k => k.TipoDescuento == tipoDescuento);

            //Cargar los tipos de descuentos
            var cupones = await query.ToListAsync();
            return View("Index", cupones);
        }
        catch (Exception e)
        {
            var errorAlert = new Alert { Type = "danger", Message = "Error al realizar la busqueda" };
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(errorAlert);
            _logger.LogError(e.Message);
            return View("Index");
        }
    }
}