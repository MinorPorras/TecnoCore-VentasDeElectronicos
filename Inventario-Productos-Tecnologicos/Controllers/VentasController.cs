using System.Security.Claims;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con las ventas del sistema.
/// </summary>
public class VentasController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<VentasController> _logger;

    public VentasController(TecnoCoreDbContext context, ILogger<VentasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Muestra la vista principal de ventas.
    /// </summary>
    /// <returns>La vista Index de ventas.</returns>
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Carro_Compras(List<TECO_A_Producto> listaCompras)
    {
        return View();
    }
    
    public async Task<IActionResult> AddToCart([FromBody] addToCartRequestViewModel model)
    {
        // Validar el modelo recibido
        if (!ModelState.IsValid || model.productId == null || model.quantity <= 0)
        {
            _logger.LogCritical("Modelo recibido es nulo o inválido: {Model}", model);
            return Json(new { success = false, message = "Datos inválidos." });
        }
        var productId = model.productId;
        var quantity = model.quantity;
        
        _logger.LogCritical("Intentando agregar producto al carrito de compras. ProductoId: {ProductoId}, Cantidad: {Quantity}", productId, quantity);
        // Verificar si el producto existe en la base de datos
        var producto = await _context.TECO_A_Producto.FindAsync(productId);
        _logger.LogCritical("Producto encontrado: {Producto}", producto);
        if (producto == null)
        {
            // Manejar el caso en que el producto no existe
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión primero"));
            return Json(new { success = false, message = "El producto no existe." });
        }
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogCritical("ID del usuario autenticado: {UsuarioId}", usuarioId);
        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para agregar productos al carrito."));
            return Json(new { success = false, message = "Usuario no autenticado." });
        }
        
        // Verificar si el producto ya está en el carrito de compras del usuario
        var carritoExistente = await _context.TECO_P_CarritoCompras
            .FirstOrDefaultAsync(c => c.TN_ProductoId == productId && c.TN_UsuarioId == usuarioId);
        _logger.LogCritical("Producto existente en el carrito: {CarritoExistente}", carritoExistente);
        if (carritoExistente != null)
        {
            // Si el producto ya está en el carrito, actualizar la cantidad
            carritoExistente.TN_Cantidad += quantity ?? 1; // Aumentar la cantidad o establecer a 1 si no se proporciona
            _logger.LogCritical("Actualizando cantidad del producto en el carrito. Nueva cantidad: {NuevaCantidad}", carritoExistente.TN_Cantidad);
            _context.TECO_P_CarritoCompras.Update(carritoExistente);
        }
        else
        {
            // Crear una nueva entrada en el carrito de compras
            var carritoCompra = new TECO_P_CarritoCompras
            {
                TN_ProductoId = productId,
                TN_Cantidad = quantity ?? 1, // Puedes ajustar la cantidad según sea necesario
                TN_UsuarioId = usuarioId, // Obtener el ID del usuario autenticado
                TN_PrecioUnitario = producto.TN_Precio
            };
            _logger.LogCritical("Creando nueva entrada en el carrito de compras: {CarritoCompra}", carritoCompra);
            // Agregar el producto al carrito de compras
            _context.TECO_P_CarritoCompras.Add(carritoCompra);
        }
        // Guardar los cambios en la base de datos
        await _context.SaveChangesAsync();
        
        // Retornar la cantidad de productos en el carrito de compras (Cuenta solo productos únicos, si hay duplicados no los cuenta)
        var cantidadCarrito = await _context.TECO_P_CarritoCompras
            .Where(c => c.TN_UsuarioId == usuarioId)
            .CountAsync();
        
        //Retornar el precio total de los productos en el carrito de compras
        var totalCarrito = await _context.TECO_P_CarritoCompras
            .Where(c => c.TN_UsuarioId == usuarioId)
            .SumAsync(c => c.TN_Cantidad * c.Producto.TN_Precio);
        
        TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("Producto agregado correctamente."));
        
        return Json(new
        {
            success = true, 
            message = "Producto agregado al carrito de compras.",
            cartItemCount = cantidadCarrito,
            cartTotal = totalCarrito
        });
    }


    public IActionResult ConfirmarCompra(List<TECO_A_Producto> listaCompras)
    {
        // Aquí se puede implementar la lógica para confirmar la compra
        // Por ejemplo, guardar la compra en una base de datos o procesar el pago

        // Redirigir a una vista de confirmación o al índice de ventas
        return RedirectToAction("Index");
    }
}