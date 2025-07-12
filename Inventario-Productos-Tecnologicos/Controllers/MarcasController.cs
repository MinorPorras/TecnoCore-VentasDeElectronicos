using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class MarcasController : Controller
{
    private readonly TecnoCoreDbContext _context;

    public MarcasController(TecnoCoreDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var marcas = await _context.TECO_M_Marca.ToListAsync();
            return View(marcas);
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar las marcas"));
            return RedirectToAction("Index", "Home");
        }
    }

    public ViewResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("TC_Nombre", "TB_Activo")] TECO_M_Marca marca)
    {
        if (!ModelState.IsValid)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Los datos ingresados no son válidos"));
            return View(marca);
        }

        try
        {
            _context.TECO_M_Marca.Add(marca);
            _context.SaveChanges();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al crear la marca"));
            return View(marca);
        }
    }

    public async Task<IActionResult> Search(string searchElement, string activeFilter)
    {
        try
        {
            ViewBag.SearchString = searchElement;
            ViewBag.ActiveFilter = activeFilter;

            var query = _context.TECO_M_Marca.AsQueryable();

            if (!string.IsNullOrEmpty(searchElement))
                query = query.Where(m => m.TC_Nombre.Contains(searchElement)
                                         || m.TN_Id.ToString().Contains(searchElement));

            if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
            {
                var isActive = activeFilter == "true";
                query = query.Where(m => m.TB_Activo == isActive);
            }

            var marcas = await query.ToListAsync();

            if (!marcas.Any())
                TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.InfoAlert("No se encontraron marcas con los criterios especificados"));

            return View("Index", marcas);
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al buscar marcas"));
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var marca = await _context.TECO_M_Marca.FindAsync(id);
        if (marca == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("la marca"));
            return RedirectToAction(nameof(Index));
        }

        return View(marca);
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] TECO_M_Marca marca)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Los datos ingresados no son válidos"));
                return BadRequest();
            }

            var marcaExistente = await _context.TECO_M_Marca.FindAsync(marca.TN_Id);
            if (marcaExistente == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("la marca"));
                return NotFound();
            }

            marcaExistente.TC_Nombre = marca.TC_Nombre;
            marcaExistente.TB_Activo = marca.TB_Activo;

            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return Ok();
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al actualizar la marca"));
            return StatusCode(500);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        try
        {
            var marca = await _context.TECO_M_Marca.FindAsync(id);
            if (marca == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("la marca"));
                return RedirectToAction(nameof(Index));
            }

            marca.TB_Activo = !marca.TB_Activo;
            _context.TECO_M_Marca.Update(marca);
            await _context.SaveChangesAsync();

            TempData["success"] =
                System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("Estado de la marca cambiado correctamente"));
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cambiar el estado de la marca"));
            return RedirectToAction(nameof(Index));
        }
    }
}