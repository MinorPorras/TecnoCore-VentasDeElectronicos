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

    private readonly Dictionary<string, string> _tipoDescuento = new()
    {
        { "1", "Porcentaje" },
        { "2", "Fijo" }
    };
    //TODO No se está cargando el tipo de descuento en la vista Create, Edit y Index.

    public CuponesController(TecnoCoreDbContext context)
    {
        _context = context;
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
        [Bind("Id,Codigo,Descripcion,TipoDescuento,ValorDescuento,FechaInicio,FechaFin,Activo")] Cupones cupon)
    {
        if (ModelState.IsValid)
        {
            _context.Add(cupon);
            await _context.SaveChangesAsync();
            var alert = new Alert { Type = "success", Message = "Cupón creado exitosamente" };
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(alert);
            return RedirectToAction(nameof(Index));
        }

        var errorAlert = new Alert { Type = "danger", Message = "Por favor, revise los datos ingresados" };
        TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(errorAlert);
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

        var tipoDescuentoKey = cupon.TipoDescuento == "Porcentaje" ? "1" : "2";
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

            // Convertir el tipo de descuento
            cupon.TipoDescuento = cupon.TipoDescuento switch
            {
                "1" => "Porcentaje",
                "2" => "Fijo",
                _ => cupon.TipoDescuento
            };

            // Validar valor según tipo de descuento
            if (cupon.TipoDescuento == "Porcentaje" && (cupon.Valor < 0 || cupon.Valor > 100))
                return BadRequest(new
                {
                    status = "error",
                    message = "El porcentaje debe de estar entre 0 y 100.",
                    type = "danger"
                });
            if (cupon.TipoDescuento == "Fijo" && cupon.Valor <= 0)
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
}

//TODO Arreglar el CRUD de cupones y crear el sistema de busqueda