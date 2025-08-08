using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador para la gestión de cupones.
/// </summary>
public class CuponesController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CuponesController> _logger;

    private readonly Dictionary<string, string> _tipoDescuento = new()
    {
        { "P", "Porcentaje" },
        { "M", "Monto" }
    };

    /// <summary>
    /// Constructor de la clase CuponesController.
    /// </summary>
    /// <param name="context">Contexto de la base de datos.</param>
    /// <param name="logger">Instancia del logger.</param>
    public CuponesController(TecnoCoreDbContext context, ILogger<CuponesController> logger)
    {
        _context = context;
        _logger = logger;
    }
    /// <summary>
    /// Muestra la lista de cupones.
    /// </summary>

    [Authorize(Roles = "Administrador")]
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
    
    /// <summary>
    /// Método privado para cargar los datos necesarios en los ViewBag.
    /// </summary>
    private void CargarViewBags()
    {
        ViewBag.TipoDescuento = new SelectList(new[]
        {
            new { Value = "P", Text = "Porcentaje (%)" },
            new { Value = "M", Text = "Monto Fijo (₡)" }
        }, "Value", "Text");
    }

    /// <summary>
    /// Muestra el formulario para crear un nuevo cupón.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public IActionResult Create()
    {
         CargarViewBags(); // Llamamos al método para preparar los datos
         var cupon = new TECO_M_Cupon()
         {
             TB_Activo = true
         };
         return View(cupon);
    }

    /// <summary>
    /// Carga los datos necesarios en los ViewBag en caso de error durante la creación.
    /// </summary>
    /// <param name="errorMessage">Mensaje de error a mostrar.</param>
    /// <param name="cupon">Objeto cupón con los datos ingresados.</param>
    private void cargarDataErrorOnCreate(string errorMessage, TECO_M_Cupon cupon)
    {
        ViewBag.TipoDescuento = new SelectList(_tipoDescuento, "Key", "Value", cupon.TC_TipoDescuento);
        ViewBag.Alert = Alert.ErrorAlert(errorMessage);
    }

    /// <summary>
    /// Crea un nuevo cupón.
    /// </summary>
    /// <param name="cupon">Objeto cupón a crear.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(TECO_M_Cupon cupon)
    {
        if (!ModelState.IsValid)
        {
            cargarDataErrorOnCreate("Por favor, revise los datos ingresados", cupon);
            return View(cupon);
        }
        try
        {
            var existingCupon = _context.TECO_M_Cupon.Where(c => c.TC_Codigo == cupon.TC_Codigo).FirstOrDefault();
            if (existingCupon != null)
            {
                cargarDataErrorOnCreate("Ya hay un cupón con ese código", cupon);
                return View(cupon);
            }

            if (cupon.TF_FechaInicio > cupon.TF_FechaFin)
            {
                cargarDataErrorOnCreate("La fecha de inicio no puede ser posterior a la fecha de finalización", cupon);
                return View(cupon);
            }

            if (cupon.TN_Valor <= 0 || cupon.TN_UsosMaximos <= 0)
            {
                cargarDataErrorOnCreate("El valor y los usos máximos deben ser mayores que cero", cupon);
                return View(cupon);
            }

            if (cupon is { TC_TipoDescuento: "P", TN_Valor: < 0 or > 100 })
            {
                cargarDataErrorOnCreate("El valor del porcentaje del decuento debe de ser mayor 0 o menor a 100", cupon);
                return View(cupon);
            }
            _context.Add(cupon);
            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cupón");
            cargarDataErrorOnCreate($"Error al crear el cupón: {ex.Message}", cupon);
            return View(cupon);
        }
    }

    /// <summary>
    /// Muestra el formulario para editar un cupón existente.
    /// </summary>
    /// <param name="id">ID del cupón a editar.</param>
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id)
    {
        Console.WriteLine($"Edit method called with id: {id}");
        var cupon = await _context.TECO_M_Cupon.FindAsync(id);
        if (cupon == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("el cupón"));
            return NotFound(new { success = false, message = "No se encontró la marca a modificar"});
        }

        ViewBag.TipoDescuento = new SelectList(_tipoDescuento, "Key", "Value", cupon.TC_TipoDescuento);
        return View(cupon);
    }

    /// <summary>
    /// Edita un cupón existente.
    /// </summary>
    /// <param name="cupon">Objeto cupón con los datos actualizados.</param>
    [HttpPut]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit([FromBody] TECO_M_Cupon cupon)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Alert = Alert.ErrorAlert("Los datos ingresados no son válidos");
                return BadRequest(new { success = false, message = "Los datos ingresados no son válidos"});
            }

            var cuponExistente = await _context.TECO_M_Cupon.FindAsync(cupon.TN_Id);
            if (cuponExistente == null)
            {
                ViewBag.Alert =  Alert.NotFoundAlert("el cupón");
                return NotFound(new { success = false, message = "No se encontró el cupón a modificar"});
            }
            
            var codCuponExistente = await _context.TECO_M_Cupon.Where( c => c.TC_Codigo == cupon.TC_Codigo).FirstOrDefaultAsync();
            if (codCuponExistente != null)
            {
                if (codCuponExistente.TN_Id != cuponExistente.TN_Id)
                {
                    ViewBag.Alert = Alert.ErrorAlert("Ya existe un cupón con ese código.");
                    return BadRequest(new { success = false, message = "Ya existe un cupón con ese código."});
                }
            }
            
            if (cupon.TF_FechaInicio > cupon.TF_FechaFin)
            {
                cargarDataErrorOnCreate("La fecha de inicio no puede ser posterior a la fecha de finalización", cupon);
                return BadRequest(new { success = false, message = "La fecha de inicio no puede ser posterior a la fecha de finalización."});
            }
            
            if (cupon is { TC_TipoDescuento: "P", TN_Valor: < 0 or > 100 })
            {
                cargarDataErrorOnCreate("El valor del porcentaje del decuento debe de ser mayor 0 o menor a 100", cupon);
                return BadRequest(new { success = false, message = "El valor del porcentaje del descuento debe de ser mayor 0 o menor a 100."});
            }

            if (cupon.TN_UsosMaximos < 1)
            {
                cargarDataErrorOnCreate("El valor de usos máximos debe de ser mayor a 0", cupon);
                return BadRequest(new { success = false, message = "El valor de usos máximos debe de ser mayor a 0."});
            }

            if (cupon.TN_UsosActuales < 0)
            {
                cargarDataErrorOnCreate("El valor de usos actuales debe de ser mayor o igual a 0", cupon);
                return BadRequest(new { success = false, message = "El valor de usos actuales debe de ser mayor o igual a 0."});
            }
            
            if (cupon.TN_UsosActuales > cupon.TN_UsosMaximos)
            {
                cargarDataErrorOnCreate("El número de usos actuales no puede ser mayor o igual al número de usos máximos", cupon);
                return BadRequest(new { success = false, message = "El número de usos actuales no puede ser mayor o igual al número de usos máximos."});
            }

            // Actualizar las propiedades del cupón existente
            // Copia los valores del objeto 'cupon' (del request) al objeto 'cuponExistente' (rastreado por el context).
            _context.Entry(cuponExistente).CurrentValues.SetValues(cupon);
            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return Ok(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CuponExists(cupon.TN_Id))
            {
                ViewBag.Alert = Alert.NotFoundAlert("el cupón");
                return NotFound(new { success = false, message = "No se encontró el cupón a modificar"});
            }

            ViewBag.Alert = Alert.ErrorAlert($"Error al actualizar el cupón");
            return StatusCode(500, new { success = false, message = $"Error interno del servidor: Error al guardar los cambios" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cupón {CuponId}", cupon.TN_Id);
            ViewBag.Alert = Alert.ErrorAlert($"Error al actualizar el cupón: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Error interno del servidor: Error al guardar los cambios" });
        }
    }

    /// <summary>
    /// Cambia el estado de actividad de un cupón (activo/inactivo).
    /// </summary>
    /// <param name="id">ID del cupón a cambiar de estado.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
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
    
    /// <summary>
    /// Verifica si un cupón existe en la base de datos.
    /// </summary>
    /// <param name="id">ID del cupón a verificar.</param>
    /// <returns>True si el cupón existe, false en caso contrario.</returns>
    private bool CuponExists(int id)
    {
        return _context.TECO_M_Cupon.Any(e => e.TN_Id == id);
    }

    /// <summary>
    /// Busca cupones según criterios de búsqueda y filtro de estado.
    /// </summary>
    [Authorize(Roles = "Administrador")]
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