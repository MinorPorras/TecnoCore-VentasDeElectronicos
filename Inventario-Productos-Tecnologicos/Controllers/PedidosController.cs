using System.Security.Claims;
using System.Text.Json;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Inventario_Productos_Tecnologicos.Viewcomponent;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con los pedidos del sistema.
/// </summary>
public class PedidosController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CarritoComprasViewComponent> _logger;
    private readonly UserManager<TECO_A_Usuario> _userManager;

    public PedidosController(TecnoCoreDbContext context, ILogger<CarritoComprasViewComponent> logger,
        UserManager<TECO_A_Usuario> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    /// <summary>
    /// Muestra la vista principal de pedidos.
    /// </summary>
    /// <returns>La vista Index de pedidos.</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Muestra la vista de pedidos específicos de un usuario.
    /// </summary>
    /// <returns>La vista de pedidos filtrada por el usuario especificado.</returns>
    public IActionResult Pedidos_Usuario()
    {
        // Obtener el ID del usuario
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("No se pudo obtener el ID del usuario.");
            TempData["Error"] = JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para eliminar productos del carrito."));
            return RedirectToAction("Informacion_Personal", "Usuario");
        }
        try
        {
            // Filtrar los pedidos por el ID del usuario
            var pedidos = _context.TECO_P_Pedido
                .Where(p => p.TN_UsuarioId == userId)
                .Include(p => p.Cupon)
                .Include(p => p.EstadoPedido)
                .Include(p => p.MetodoPago)
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Direccion)
                .ToList();
            _logger.LogInformation("Cantidad de pedidos encontrados: {Count}", pedidos.Count);
            // Verificar si se encontraron pedidos, si se encontraron, se retorna la vista con los pedidos
            if (pedidos.Count != 0) return View(pedidos);
            _logger.LogInformation("No se encontraron pedidos para el usuario con ID: {UserId}", userId);
            ViewBag.Alert = JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para eliminar productos del carrito."));
            return RedirectToAction("Informacion_Personal", "Usuario");

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener los pedidos del usuario con ID: {UserId}", userId);
            TempData["Error"] = JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para eliminar productos del carrito."));
            return RedirectToAction("Informacion_Personal", "Usuario");
        }
    }

    public IActionResult Estados_Pedidos()
    {
        return View();
    }
}