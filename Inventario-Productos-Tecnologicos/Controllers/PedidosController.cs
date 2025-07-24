using System.Security.Claims;
using System.Text.Json;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Inventario_Productos_Tecnologicos.Viewcomponent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    /// Muestra la vista principal de pedidos mostrando únicamente los pedidos en cualquier estado menos los completados.
    /// </summary>
    /// <returns>La vista Index de pedidos.</returns>
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Index()
    {
        try
        {
            // Obtener los pedidos que no están en estado completado
            var pedidos = await _context.TECO_P_Pedido
                .Include(p => p.Cupon)
                .Include(p => p.EstadoPedido)
                .Include(p => p.MetodoPago)
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Direccion)
                .Where(p => p.TN_EstadoPedidoId != 5) // Excluir pedidos completados
                .ToListAsync();
            var estadosPedidos = new SelectList(await _context.TECO_M_EstadoPedido.ToListAsync(),
                "TN_Id", "TC_NombreEstado");
            ViewBag.estadosPedidos = estadosPedidos;
            ViewBag.selectedEstadoBusqueda = 0;
            ViewBag.searchTerm = "";

            return View(pedidos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener los pedidos.");
            return View();
        }
    }

    /// <summary>
    /// Muestra la vista de pedidos específicos de un usuario.
    /// </summary>
    /// <returns>La vista de pedidos filtrada por el usuario especificado.</returns>
    [Authorize (Roles = "Cliente, Administrador")]
    public async Task<IActionResult> Pedidos_Usuario()
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
            var pedidos = await _context.TECO_P_Pedido
                .Where(p => p.TN_UsuarioId == userId)
                .Include(p => p.Cupon)
                .Include(p => p.EstadoPedido)
                .Include(p => p.MetodoPago)
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Direccion)
                .ToListAsync();
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
    
    /// <summary>
    /// Realiza la búsqueda de pedidos basándose en el término de búsqueda y el estado del pedido.
    /// Solo accesible para usuarios con el rol "Administrador".
    /// </summary>
    /// <param name="searchTerm">Término para buscar en código de pedido o nombre de cliente.</param>
    /// <param name="estadoPedidoBusqueda">ID del estado de pedido para filtrar.</param>
    /// <returns>La vista Index con los pedidos filtrados.</returns>
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> SearchPedidos(string? searchTerm, int? estadoPedidoBusqueda)
    {
        try
        {
            // Inicializa la consulta base con todas las inclusiones necesarias
            IQueryable<TECO_P_Pedido> query = _context.TECO_P_Pedido
                .Include(p => p.Cupon)
                .Include(p => p.EstadoPedido)
                .Include(p => p.MetodoPago)
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Direccion);

            // Filtro por estado del pedido (si se seleccionó uno)
            if (estadoPedidoBusqueda.HasValue && estadoPedidoBusqueda.Value != 0) // Asumo 0 o null si no se seleccionó
            {
                query = query.Where(p => p.TN_EstadoPedidoId == estadoPedidoBusqueda.Value);
            }
            else
            {
                // Si no se selecciona un estado específico, mantener la exclusión de "completados"
                query = query.Where(p => p.TN_EstadoPedidoId != 5);
            }

            // Filtro por término de búsqueda (si se proporcionó uno)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var upperSearchTerm = searchTerm.ToUpper().Trim(); // Convertir a mayúsculas para la comparación LIKE

                query = query.Where(p =>
                    (p.TN_TransaccionId != null && EF.Functions.Like(p.TN_TransaccionId.ToUpper(), $"%{upperSearchTerm}%")) ||
                    (p.Usuario != null && EF.Functions.Like(p.Usuario.TC_Nombre.ToUpper(), $"%{upperSearchTerm}%")) ||
                    (p.Usuario != null && EF.Functions.Like(p.Usuario.TC_Apellidos.ToUpper(), $"%{upperSearchTerm}%")));
            }

            var pedidos = await query.ToListAsync();

            // Carga los estados de pedido para el DropDownList en la vista de resultados
            var estadosPedidos = new SelectList(await _context.TECO_M_EstadoPedido.ToListAsync(),
                "TN_Id", "TC_NombreEstado", estadoPedidoBusqueda); // Pasar el valor seleccionado
            ViewBag.estadosPedidos = estadosPedidos;

            ViewBag.searchTerm = searchTerm;
            
            ViewBag.selectedEstadoBusqueda = estadoPedidoBusqueda;

            // Retorna la misma vista Index con los resultados filtrados
            return View("Index", pedidos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al buscar pedidos con término '{SearchTerm}' y estado '{EstadoId}'.", searchTerm, estadoPedidoBusqueda);
            TempData["Error"] = JsonSerializer.Serialize(
                Alert.ErrorAlert("Ocurrió un error al realizar la búsqueda de pedidos."));
            // En caso de error, puedes redirigir a la vista Index sin filtros o a una página de error
            return RedirectToAction(nameof(Index));
        }
    }
    
            /// <summary>
    /// Realiza la búsqueda de pedidos basándose en el término de búsqueda y el estado del pedido.
    /// Solo accesible para usuarios con el rol "Administrador".
    /// </summary>
    /// <param name="searchTerm">Término para buscar en código de pedido o nombre de cliente.</param>
    /// <param name="estadoPedidoBusqueda">ID del estado de pedido para filtrar.</param>
    /// <returns>La vista Index con los pedidos filtrados.</returns>
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> SearchPedidosCompletados(string? searchTerm, int? estadoPedidoBusqueda)
    {
        try
        {
            // Inicializa la consulta base con todas las inclusiones necesarias
            IQueryable<TECO_P_Pedido> query = _context.TECO_P_Pedido
                .Include(p => p.Cupon)
                .Include(p => p.EstadoPedido)
                .Include(p => p.MetodoPago)
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Direccion);

            // Filtro por término de búsqueda (si se proporcionó uno)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var upperSearchTerm = searchTerm.ToUpper().Trim(); // Convertir a mayúsculas para la comparación LIKE

                query = query.Where(p =>
                    (p.TN_TransaccionId != null &&
                     EF.Functions.Like(p.TN_TransaccionId.ToUpper(), $"%{upperSearchTerm}%")) ||
                    (p.Usuario != null && EF.Functions.Like(p.Usuario.TC_Nombre.ToUpper(), $"%{upperSearchTerm}%")) ||
                    (p.Usuario != null &&
                     EF.Functions.Like(p.Usuario.TC_Apellidos.ToUpper(), $"%{upperSearchTerm}%")) &&
                    (p.TN_EstadoPedidoId == 5) // Asegurarse de que solo se busquen pedidos completados
                );
            }
            else
            {
                query = query.Where(p => p.TN_EstadoPedidoId == 5); // Filtrar solo pedidos completados
            }

            var pedidos = await query.ToListAsync();

            ViewBag.searchTerm = searchTerm;
            
            ViewBag.estad = searchTerm;
            
            // Retorna la misma vista Index con los resultados filtrados
            return View("Pedidos_completados", pedidos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al buscar pedidos con término '{SearchTerm}' y estado '{EstadoId}'.", searchTerm, estadoPedidoBusqueda);
            TempData["Error"] = JsonSerializer.Serialize(
                Alert.ErrorAlert("Ocurrió un error al realizar la búsqueda de pedidos."));
            // En caso de error, puedes redirigir a la vista Index sin filtros o a una página de error
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarEstado(int pedidoId, int estadoPedido)
    {
        _logger.LogCritical("ID del pedido: {PedidoId}, Estado del pedido: {EstadoPedido}", pedidoId, estadoPedido);
        // Validar la existencia del pedido
        var pedido = await _context.TECO_P_Pedido
            .Include(p => p.EstadoPedido)
            .FirstOrDefaultAsync(p => p.TN_Id == pedidoId);
        if (pedido == null)
        {
            _logger.LogCritical("El pedido con ID {PedidoId} no existe.", pedidoId);
            TempData["Error"] = JsonSerializer.Serialize(
                Alert.ErrorAlert("El pedido no existe o ha sido eliminado."));
            return RedirectToAction("Index");
        }
        // Validar el estado del pedido
        if (pedido.TN_EstadoPedidoId == 5)
        {
            _logger.LogInformation("El pedido con ID {PedidoId} ha sido entregado y su estado no puede ser modificado.", pedidoId);
            TempData["Success"] = JsonSerializer.Serialize(
                Alert.InfoAlert("El pedido ha sido completado exitosamente."));
        }
        else
        {
            // Actualizar el estado del pedido
            pedido.TN_EstadoPedidoId = estadoPedido;
            await _context.SaveChangesAsync();
            _logger.LogInformation("El estado del pedido con ID {PedidoId} ha sido actualizado a {EstadoPedido}.",
                pedidoId, estadoPedido);
            TempData["Success"] = JsonSerializer.Serialize(
                Alert.InfoAlert("El estado del pedido ha sido actualizado exitosamente."));
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Pedidos_completados()
    {
        //Obtener todos los pedidos ya completados
        var pedidosCompletados = await _context.TECO_P_Pedido
            .Include(p => p.Cupon)
            .Include(p => p.EstadoPedido)
            .Include(p => p.MetodoPago)
            .Include(p => p.DetallePedidos)
            .ThenInclude(dp => dp.Producto)
            .Include(p => p.Usuario)
            .ThenInclude(u => u.Direccion)
            .Where(p => p.TN_EstadoPedidoId == 5) // Filtrar por estado completado
            .ToListAsync();
        if (pedidosCompletados.Count == 0)
        {
            _logger.LogInformation("No se encontraron pedidos completados.");
            TempData["Info"] = JsonSerializer.Serialize(
                Alert.InfoAlert("No hay pedidos completados en el sistema."));
        }
        else
        {
            _logger.LogInformation("Cantidad de pedidos completados encontrados: {Count}", pedidosCompletados.Count);
        }
        return View(pedidosCompletados);

    }
}