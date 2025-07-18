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
        
        // Obtener la nueva lista de productos en el carrito de compras
        var productosEnCarrito = await _context.TECO_P_CarritoCompras
            .Include(c => c.Producto) // Incluir el objeto Producto relacionado
            .Where(c => c.TN_UsuarioId == usuarioId)
            .ToListAsync();
        
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
            cartTotal = totalCarrito.ToString("N2"), // Formatear el total a dos decimales
            cartItems = productosEnCarrito.Select(item => new
            {
                productId = item.TN_ProductoId,
                productName = item.Producto.TC_Nombre,
                productPrice = item.TN_PrecioUnitario.ToString("N2"), // Formatear el precio a dos decimales
                quantity = item.TN_Cantidad,
                totalPrice = (item.TN_Cantidad * item.TN_PrecioUnitario).ToString("N2"), // Calcular el precio total del producto
                deleteImage = Url.Content("~/img/ICO_Delete.svg") // Or if each product has its own image, use item.Producto.TC_Imagen
            })
        });
    }

    public async Task<IActionResult> GetCartItems()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _logger.LogCritical("ID del usuario autenticado: {UsuarioId}", usuarioId);
        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para ver los productos del carrito."));
            return Json(new { success = false, message = "Usuario no autenticado." });
        }
        
        try
        {
            // Obtener los productos en el carrito de compras del usuario autenticado
            // Ensure Producto is included and handled if it might be null for some reason
            var productosEnCarrito = await _context.TECO_P_CarritoCompras
                .Include(c => c.Producto) // Incluir el objeto Producto relacionado
                .Where(c => c.TN_UsuarioId == usuarioId)
                .ToListAsync();

            // Initialize defaults in case the cart is empty
            int cantidadCarrito = 0;
            decimal totalCarrito = 0m;
            var cartItemsData = new List<object>();

            if (productosEnCarrito.Count != 0)
            {
                cantidadCarrito = productosEnCarrito.Count;

                totalCarrito = productosEnCarrito.Sum(c => c.TN_Cantidad * (c.Producto?.TN_Precio ?? 0m));

                cartItemsData = productosEnCarrito.Select(item => new
                {
                    productId = item.TN_ProductoId,
                    productName = item.Producto?.TC_Nombre ?? "Producto Desconocido",
                    productPrice = item.TN_PrecioUnitario,
                    quantity = item.TN_Cantidad,
                    totalPrice = (item.TN_Cantidad * (item.Producto?.TN_Precio ?? 0m)),
                    deleteImage = Url.Content("~/img/ICO_Delete.svg")
                }).ToList<object>(); 
            }
        
            _logger.LogCritical("Cantidad de productos en el carrito: {CantidadCarrito}", cantidadCarrito);
            _logger.LogCritical("Precio total de los productos en el carrito: {TotalCarrito}", totalCarrito);

            return Json(new
            {
                success = true,
                cartItemCount = cantidadCarrito,
                cartTotal = totalCarrito, // Send as decimal, let JS format for display
                cartItems = cartItemsData // Always send a list, even if empty
            });
        }
        catch (Exception ex)
        {
            // Log the full exception details on the server
            _logger.LogError(ex, "Error getting cart items for user {UserId}", usuarioId);
            // Return a consistent error JSON structure to the client
            return StatusCode(500, new { success = false, message = "Error interno del servidor al obtener el carrito." });
        }
    }

    public async Task<IActionResult> DeleteCartItem([FromBody] DeleteProductCartViewModel model)
    {
        _logger.LogCritical("Intentando eliminar producto del carrito de compras. ProductoId: {ProductoId}", model.productId);
        
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para eliminar productos del carrito."));
            return Json(new { success = false, message = "Usuario no autenticado." });
        }
        
        // Verificar si el carrito de compras del usuario autenticado tiene productos
        var carritoCompras = await _context.TECO_P_CarritoCompras
            .Where(c => c.TN_UsuarioId == usuarioId)
            .ToListAsync();
        
        //_logger.LogCritical("Productos en el carrito antes de eliminar: {CarritoCompras}", carritoCompras);
        
        if (carritoCompras.Count == 0)
        {
            _logger.LogCritical("El carrito de compras está vacío.");
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("El carrito de compras está vacío."));
            return Json(new { success = false, message = "El carrito de compras está vacío." });
        }
        
        // Eliminar el producto del carrito de compras del usuario autenticado
        var productoAEliminar = await _context.TECO_P_CarritoCompras
            .FirstOrDefaultAsync(c => c.TN_UsuarioId == usuarioId && c.TN_ProductoId == model.productId);
        //_logger.LogCritical("Producto a eliminar del carrito: {ProductoAEliminar}", productoAEliminar);
        
        if (productoAEliminar != null)
        {
            _context.TECO_P_CarritoCompras.Remove(productoAEliminar);
            await _context.SaveChangesAsync();
            _logger.LogCritical("Producto eliminado del carrito de compras exitosamente.");
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("Producto eliminado del carrito de compras correctamente."));

            return Json(new
            {
                success = true,
                message = "Producto eliminado del carrito de compras correctamente.",
            });
        }
        else
        {
            _logger.LogCritical("El producto no se encontró en el carrito de compras.");
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("El producto no se encontró en el carrito de compras."));
            return Json(new
            {
                success = false,
                message = "No se encontró el producto.",
            });
        }
    }

    public async Task<IActionResult> EmptyCart()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogCritical("ID del usuario autenticado: {UsuarioId}", usuarioId);
        
        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para vaciar el carrito."));
            return Json(new { success = false, message = "Usuario no autenticado." });
        }
        
        // Eliminar todos los productos del carrito de compras del usuario autenticado
        var carritoCompras = await _context.TECO_P_CarritoCompras
            .Where(c => c.TN_UsuarioId == usuarioId)
            .ToListAsync();
        _logger.LogCritical("Productos en el carrito antes de vaciar: {CarritoCompras}", carritoCompras);
        
        if (carritoCompras.Count > 0)
        {
            _context.TECO_P_CarritoCompras.RemoveRange(carritoCompras);
            await _context.SaveChangesAsync();
            _logger.LogCritical("Carrito de compras vaciado exitosamente.");
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("Carrito de compras vaciado correctamente."));
        }
        else
        {
            _logger.LogCritical("El carrito de compras ya estaba vacío.");
            TempData["info"] = System.Text.Json.JsonSerializer.Serialize(Alert.InfoAlert("El carrito de compras ya estaba vacío."));
        }
        
        // Retornar la cantidad de productos en el carrito de compras (Cuenta solo productos únicos, si hay duplicados no los cuenta)
        var cantidadCarrito = await _context.TECO_P_CarritoCompras
            .Where(c => c.TN_UsuarioId == usuarioId)
            .CountAsync();
        _logger.LogCritical("Cantidad de productos en el carrito después de vaciar: {CantidadCarrito}", cantidadCarrito);
        
        //Retornar el precio total de los productos en el carrito de compras
        var totalCarrito = await _context.TECO_P_CarritoCompras
            .Where(c => c.TN_UsuarioId == usuarioId)
            .SumAsync(c => c.TN_Cantidad * c.Producto.TN_Precio);
        _logger.LogCritical("Precio total de los productos en el carrito después de vaciar: {TotalCarrito}", totalCarrito);
        
        return Json(new
        {
            success = true,
            message = "Carrito de compras vaciado correctamente.",
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