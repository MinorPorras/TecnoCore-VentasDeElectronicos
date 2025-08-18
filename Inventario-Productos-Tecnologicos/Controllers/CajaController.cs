using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Identity; // Necesario para UserManager y SignInManager
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models; // Tu modelo Usuarios, Provincia, Canton, Direccion
using Inventario_Productos_Tecnologicos.Models.ViewModels; // Tu RegisterViewModel
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Authorization; // Tu DbContext
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using Microsoft.EntityFrameworkCore;
namespace Inventario_Productos_Tecnologicos.Controllers;

public class CajaController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CajaController> _logger;
    private readonly UserManager<TECO_A_Usuario> _userManager;

    public CajaController(TecnoCoreDbContext context, ILogger<CajaController> logger, UserManager<TECO_A_Usuario> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Index()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var itemsCaja = await _context.TECO_P_CarritoCompras
            .Include(i => i.Producto)
            .Where(i => i.TN_UsuarioId == usuarioId)
            .Where(i => i.Producto != null)
            .ToListAsync();
        return View(itemsCaja);
    }
    
    [Authorize(Roles = "Administrador")]
    public async Task<JsonResult> AddProductToCaja([FromBody] addToCartRequestViewModel model)
    {
        // Validar el modelo recibido
        if (!ModelState.IsValid || model.quantity <= 0)
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
            ViewBag.Alert = JsonSerializer.Serialize(
                Alert.ErrorAlert("El producto no existe."));
            return Json(new { success = false, message = "El producto no existe." });
        }

        if (producto.TN_Stock <= 0)
        {
            // Manejar el caso en que el producto no tiene el stock suficiente
            ViewBag.Alert = JsonSerializer.Serialize(
                Alert.ErrorAlert("El producto no tiene stock suficiente para ejecutar esta acción"));
            return Json(new { success = false, message = "El producto no tiene stock suficiente para ejecutar esta acción" });
        }
        
        
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogCritical("ID del usuario autenticado: {UsuarioId}", usuarioId);
        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = JsonSerializer.Serialize(
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
        
        return Json(new
        {
            success = true, 
            message = "Producto agregado al carrito de compras.",
            cartItemCount = cantidadCarrito,
            cajaTotal = totalCarrito,
            cajaItems = productosEnCarrito.Select(item => new
            {
                productId = item.TN_ProductoId,
                productName = item.Producto.TC_Nombre,
                productPrice = item.TN_PrecioUnitario,
                quantity = item.TN_Cantidad,
                totalPrice = (item.TN_Cantidad * item.TN_PrecioUnitario), // Calcular el precio total del producto
                deleteImage = Url.Content("~/img/ICO_Delete.svg"),
                productMaxStock = item.Producto?.TN_Stock ?? 0,
                plusImage = Url.Content("~/img/ICO_Add.svg"), 
                minusImage = Url.Content("~/img/ICO_Minus.svg")

            })
        });
    }
    
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetCajaItems()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            _logger.LogWarning("Intento de acceder al carrito sin autenticación.");
            return Json(new { success = false, message = "Usuario no autenticado." });
        }
        
        try
        {
            // Obtener los productos en el carrito de compras del usuario autenticado
            // Ensure Producto is included and handled if it might be null for some reason
            var productosEnCarrito = await _context.TECO_P_CarritoCompras
                .Include(c => c.Producto) // Incluir el objeto Producto relacionado
                .Where(c => c.TN_UsuarioId == usuarioId)
                .Where(c => c.Producto != null)
                .ToListAsync();

            // Initialize defaults in case the cart is empty
            var cantidadItemsDistintos = productosEnCarrito.Count; // Cantidad de tipos de productos distintos
            var cantidadTotalProductos = productosEnCarrito.Sum(c => c?.TN_Cantidad ?? 0); // Suma de todas las cantidades
            
            var subtotalCarrito = 0m;
            var cartItemsData = new List<object>();

            if (productosEnCarrito.Any())
            {
                cantidadItemsDistintos = productosEnCarrito.Count;

                // Calcular el subtotal del carrito antes de aplicar cualquier descuento
                subtotalCarrito = productosEnCarrito.Sum(c => c.Producto.TN_Precio * c.TN_Cantidad);
                cartItemsData = productosEnCarrito.Select(item => new
                {
                    productId = item.TN_ProductoId,
                    productName = item.Producto?.TC_Nombre ?? "Producto Desconocido",
                    productPrice = item.Producto?.TN_Precio ?? 0m, // Usar PrecioVenta del producto
                    quantity = item.TN_Cantidad,
                    // Subtotal por item: (Cantidad * PrecioVenta del producto)
                    itemSubtotal = (item.TN_Cantidad) * (item.Producto?.TN_Precio ?? 0m),
                    deleteImage = Url.Content("~/img/ICO_Delete.svg"),
                    productMaxStock = item.Producto?.TN_Stock ?? 0,
                    plusImage = Url.Content("~/img/ICO_Add.svg"),
                    minusImage = Url.Content("~/img/ICO_Minus.svg")
                }).ToList<object>(); 
            }
            
            //Sección de revisión de descuentos
            var descuentoAplicado = 0m;
            var totalCarritoFinal = subtotalCarrito;
            var codigoCupon = string.Empty;
            
            // Verificar si hay un cupón aplicado
            int? idCuponAplicado = HttpContext.Session.GetInt32("AppliedCouponId");
            if (idCuponAplicado.HasValue)
            {
                var cupon = await _context.TECO_M_Cupon.FirstOrDefaultAsync(c => c.TN_Id == idCuponAplicado.Value && c.TB_Activo);
                if (cupon != null)
                {
                    descuentoAplicado = cupon.TC_TipoDescuento switch
                    {
                        //TODO añadir validaciones de fecha, usos, etc
                        "P" when cupon.TN_Valor > 0 => subtotalCarrito * (cupon.TN_Valor / 100m),
                        "M" when cupon.TN_Valor > 0 => cupon.TN_Valor,
                        _ => descuentoAplicado
                    };

                    // Asegurarse de que el descuento no haga que el total sea negativo
                    if (descuentoAplicado > subtotalCarrito)
                    {
                        descuentoAplicado = subtotalCarrito; //El descuento máximo es el total del carrito
                    }
                    totalCarritoFinal = subtotalCarrito - descuentoAplicado;
                    codigoCupon = cupon.TC_Codigo; // Guardar el código del cupón para mostrarlo
                }
                else
                {
                    HttpContext.Session.Remove("AppliedCouponId"); // Eliminar el cupón si no es válido
                }
            }
            
        
            _logger.LogInformation("Carrito para usuario {UserId}: Cantidad de productos: {CantidadProductos}, Subtotal: {Subtotal}, Descuento Aplicado: {Descuento}, Total Final: {TotalFinal}",
                usuarioId, cantidadTotalProductos, subtotalCarrito, descuentoAplicado, totalCarritoFinal);

            return Json(new
            {
                success = true,
                cajaItemCount = cantidadTotalProductos, // Cantidad total de productos (sumando cantidades)
                cajaTotal = totalCarritoFinal, // Este es el total FINAL (subtotal - descuento)
                subtotalCaja = subtotalCarrito, // Enviamos el subtotal original también
                descuentoAplicado,
                appliedCouponCode = codigoCupon, // Código del cupón aplicado
                cajaItems = cartItemsData
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
    
    [Authorize(Roles = "Administrador")]
    public async Task<JsonResult> EmptyCart()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogCritical("ID del usuario autenticado: {UsuarioId}", usuarioId);
        
        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = JsonSerializer.Serialize(
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
        }
        else
        {
            _logger.LogCritical("El carrito de compras ya estaba vacío.");
            TempData["info"] = JsonSerializer.Serialize(Alert.InfoAlert("El carrito de compras ya estaba vacío."));
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
        
    [Authorize(Roles = "Administrador")]
    public async Task<JsonResult> DeleteCajaItem([FromBody] DeleteProductCartViewModel model)
    {
        _logger.LogCritical("Intentando eliminar producto del carrito de compras. ProductoId: {ProductoId}", model.productId);
        
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = JsonSerializer.Serialize(
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
            ViewBag.Alert = JsonSerializer.Serialize(Alert.InfoAlert("El carrito de compras está vacío."));
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

            return Json(new
            {
                success = true,
                message = "Producto eliminado del carrito de compras correctamente.",
            });
        }
        else
        {
            _logger.LogCritical("El producto no se encontró en el carrito de compras.");
            ViewBag.Alert = JsonSerializer.Serialize(Alert.InfoAlert("El producto no se encontró en el carrito de compras."));
            return Json(new
            {
                success = false,
                message = "No se encontró el producto.",
            });
        }
    }
    
    [Authorize(Roles = "Administrador")]
    public async Task<JsonResult> DecreaseCajaItem([FromBody] addToCartRequestViewModel model) // Reuse the same ViewModel
    {
        if (!ModelState.IsValid || model.quantity <= 0)
        {
            return Json(new { success = false, message = "Datos inválidos para disminuir." });
        }

        var productId = model.productId;
        var cantidadRestar = model.quantity ?? 1; // Default a disminuir por 1

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Json(new { success = false, message = "Usuario no autenticado." });
        }

        var carritoExistente = await _context.TECO_P_CarritoCompras
            .FirstOrDefaultAsync(c => c.TN_ProductoId == productId && c.TN_UsuarioId == usuarioId);

        if (carritoExistente == null)
        {
            return Json(new { success = false, message = "Producto no encontrado en el carrito." });
        }
        if (carritoExistente.TN_Cantidad < cantidadRestar)
        {
            return Json(new { success = false, message = "No se puede restar más de la cantidad actual." });
        }
        carritoExistente.TN_Cantidad -= cantidadRestar;

        if (carritoExistente.TN_Cantidad <= 0)
        {
            // If quantity drops to 0 or below, remove the item entirely
            _context.TECO_P_CarritoCompras.Remove(carritoExistente);
        }
        
        await _context.SaveChangesAsync();
        
        // After modification, get updated cart data to send back
        // (You can reuse the logic from GetCartItems, or even call it directly if it's refactored)
        var updatedCartItems = await _context.TECO_P_CarritoCompras
            .Include(c => c.Producto)
            .Where(c => c.TN_UsuarioId == usuarioId)
            .ToListAsync();

        int newCartItemCount = updatedCartItems.Count; // This counts unique items
        decimal newCartTotal = updatedCartItems.Sum(c => c.TN_Cantidad * (c.Producto?.TN_Precio ?? 0m)); // Defensive sum

        return Json(new
        {
            success = true,
            message = carritoExistente.TN_Cantidad <= 0 ? "Producto eliminado del carrito." : "Cantidad actualizada.",
            cartItemCount = newCartItemCount,
            cartTotal = newCartTotal,
        });
    }
    
    [Authorize(Roles = "Administrador")]
    public async Task<JsonResult> ApplyDiscount([FromBody] ApplyDiscountRequestViewModel model)
    {
        // Validar el modelo recibido
        if (string.IsNullOrEmpty(model.codigoCupon) || model.totalCarrito <= 0)
        {
            return Json(new { success = false, message = "Datos de cupón inválidos." });
        }
        
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Verificar si el usuario está autenticado
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Json(new { success = false, message = "Debe iniciar sesión para aplicar cupones." });
        }

        var cupon = _context.TECO_M_Cupon
            .FirstOrDefault(c => c.TC_Codigo == model.codigoCupon && c.TB_Activo);
        
        // TODO: Añadir validaciones de fecha, usos, etc
        
        
        if (cupon == null)
        {
            ViewBag.Alert = JsonSerializer.Serialize(
                Alert.ErrorAlert("Cupón no válido o no activo."));
            return Json(new { success = false, message = "Cuón no existente" });
        }
        
        
        var cajaItems = await _context.TECO_P_CarritoCompras
            .Where(cc => cc.TN_UsuarioId == usuarioId)
            .Include(cc => cc.Producto)
            .ToListAsync();
        
        var subtotalCarrito = cajaItems.Sum(item => (item.Producto?.TN_Precio ?? 0) * (item?.TN_Cantidad ?? 0));
        decimal descuentoAplicado = 0;

        if (cajaItems.Count == 0)
        {
            return Json(new { success = false, message = "No hay productos en el carrito para aplicar el cupón." });
        }
        
        if (subtotalCarrito == 0)
        {
            ViewBag.Alert = JsonSerializer.Serialize(
                Alert.ErrorAlert("El carrito está vacío no se puede aplicar un cupón."));
            return Json(new { success = false, message = "El carrito está vacío." });
        }
        
        _logger.LogCritical("Tipo de descuento: " +  cupon.TC_TipoDescuento);

        descuentoAplicado = cupon.TC_TipoDescuento switch
        {
            // Calcular el descuento
            "P" when cupon.TN_Valor > 0 => subtotalCarrito * (cupon.TN_Valor / 100m),
            "M" when cupon.TN_Valor > 0 => cupon.TN_Valor,
            _ => descuentoAplicado
        };
        
        _logger.LogCritical("Descuento aplicado: " + descuentoAplicado);

        // Asegurarse de que el descuento no haga que el total sea negativo
        if (descuentoAplicado > subtotalCarrito)
        {
            descuentoAplicado = subtotalCarrito;
        }
        
        var totalCajaConDescuento = subtotalCarrito - descuentoAplicado;

        _logger.LogCritical("Monto total: " + subtotalCarrito + "Total con descuento: " + totalCajaConDescuento);
        // Guarda el ID del cupón en la sesión para que se use en futuras cargas del carrito
        HttpContext.Session.SetInt32("AppliedCouponId", cupon.TN_Id);

        return Json(new
        {
            success = true,
            message = "Cupón aplicado correctamente.",
            descuentoAplicado,
            totalCajaConDescuento,
            cajaItemCount = cajaItems.Sum(item => item.TN_Cantidad) // O el count de items distintos
        });
    }
    
    [Authorize(Roles = "Administrador")]
    public JsonResult RemoveDiscount()
    {
        _logger.LogCritical("Eliminando cupón aplicado de la sesión: " + HttpContext.Session.GetString("AppliedCouponId"));
        // Eliminar el cupón aplicado de la sesión
        HttpContext.Session.Remove("AppliedCouponId");
        
        _logger.LogCritical("Cupón eliminado: " + HttpContext.Session.GetString("AppliedCouponId"));

        // Retornar una respuesta indicando que el cupón fue eliminado
        return Json(new { success = true, message = "Cupón eliminado correctamente." });
    }
    
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public IActionResult GetProductToSearch(string searchTerm = "") //Valor vacío por defecto
    {
        IQueryable<TECO_A_Producto> query = _context.TECO_A_Producto;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            if (int.TryParse(searchTerm, out var searchId))
            {
                query = query.Where(p => p.TN_Id == searchId);
            }
            else
            {
                query = query.Where(p => p.TC_Nombre.ToLower().Contains(searchTerm.ToLower()));
            }
        }
        
        //Se crea una lista de productos con los datos que se necesitan
        var products = query.Select(p => new
        {
            tn_Id = p.TN_Id,
            tc_Nombre = p.TC_Nombre,
            tn_Stock = p.TN_Stock
        }).ToList();
        return Ok(products); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> TerminarCompra(string dineroEntregaCliente, string vueltoCaja)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogCritical("Modelo recibido es nulo o inválido.");
            return RedirectToAction("Index");
        }
        

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Manejar el caso en que el usuario no está autenticado
            ViewBag.Alert = JsonSerializer.Serialize(
                Alert.ErrorAlert("Debe iniciar sesión para realizar la compra."));
            return RedirectToAction("Index");
        }
        _logger.LogCritical("ID del usuario autenticado: {UsuarioId}", usuarioId);
        
        // Verificar si el carrito de compras del usuario autenticado tiene productos
        var carritoCompras = _context.TECO_P_CarritoCompras
            .Where(c => c.TN_UsuarioId == usuarioId)
            .Include(c => c.Producto) // Incluir el objeto Producto relacionado
            .ToList();
        
        if (carritoCompras.Count == 0)
        {
            ViewBag.Alert = JsonSerializer.Serialize(
                Alert.InfoAlert("El carrito de compras está vacío."));
            return RedirectToAction("Index");
        }
        _logger.LogCritical("Cantidad de productos en el carrito: {CantidadCarrito}", carritoCompras.Count);

        try
        {

            var totalCarrito = carritoCompras.Sum(c => c.TN_Cantidad * c.Producto.TN_Precio);
            _logger.LogCritical("Total del carrito antes de aplicar cupones: {TotalCarrito}", totalCarrito);
            var cuponId = HttpContext.Session.GetInt32("AppliedCouponId");
            _logger.LogCritical("ID del cupón aplicado: {CuponId}", cuponId);
            var cupon = new TECO_M_Cupon();
            if (cuponId != null)
            {
                cupon = await _context.TECO_M_Cupon.FirstOrDefaultAsync(c => c.TN_Id == cuponId && c.TB_Activo 
                    && c.TF_FechaFin >= DateTime.Now && c.TF_FechaInicio <= DateTime.Now);
                if (cupon == null)
                {
                    cupon = new TECO_M_Cupon
                    {
                        TN_Id = 0,
                        TN_Valor = 0,
                        TC_TipoDescuento = "M", // No hay descuento aplicado
                        TF_FechaInicio = DateTime.Now,
                        TF_FechaFin = DateTime.Now.AddYears(1), // Cupón sin descuento, válido por un año
                        TN_UsosMaximos = 1,
                        TN_UsosActuales = 0
                    };
                }
                else
                {
                    //Si el cupón es válido, verificar si se ha alcanzado el máximo de usos
                    if (cupon.TN_UsosActuales >= cupon.TN_UsosMaximos)
                    {
                        _logger.LogCritical("El cupón ha alcanzado el máximo de usos permitidos.");
                        TempData["error"] = JsonSerializer.Serialize(Alert.ErrorAlert("El cupón ha alcanzado el máximo de usos permitidos."));
                        return RedirectToAction("Index");
                    }
                    
                    // Incrementar el contador de usos del cupón
                    cupon.TN_UsosActuales++;
                }
            }
            else
            {
                cupon = new TECO_M_Cupon
                {
                    TN_Id = 0,
                    TN_Valor = 0,
                    TC_TipoDescuento = "M", // No hay descuento aplicado
                    TF_FechaInicio = DateTime.Now,
                    TF_FechaFin = DateTime.Now.AddYears(1), // Cupón sin descuento, válido por un año
                    TN_UsosMaximos = 1,
                    TN_UsosActuales = 0
                };
            }

            decimal descuento;

            switch (cupon.TC_TipoDescuento)
            {
                case "P" when cupon.TN_Valor > 0:
                    _logger.LogCritical("Aplicando descuento porcentual del cupón: {CuponValor}", cupon.TN_Valor);
                    descuento = totalCarrito * (cupon.TN_Valor / 100m);
                    break;
                case "M" when cupon.TN_Valor > 0:
                    _logger.LogCritical("Aplicando descuento monetario del cupón: {CuponValor}", cupon.TN_Valor);
                    descuento = cupon.TN_Valor;
                    break;
                default:
                    _logger.LogCritical("No se aplica descuento, cupón no válido o sin descuento.");
                    descuento = 0; // No hay descuento aplicado
                    break;
            }
            
            // Asegurarse de que el descuento no haga que el total sea negativo
            if (descuento > totalCarrito)
            {
                _logger.LogCritical("El descuento es mayor que el total del carrito, ajustando descuento.");
                descuento = totalCarrito; // El descuento máximo es el total del carrito
            }
            
            var totalFinal = totalCarrito - descuento;
            _logger.LogCritical("Descuento aplicado: {Descuento}", descuento);
            var transaccionId = Guid.NewGuid(); // Generar UUID única para la transacción

            var pedido = new TECO_P_Pedido
            {
                TN_UsuarioId = usuarioId,
                TN_MetodoPagoId = 1, // Asignar un método de pago por defecto en este caso 1 es para tarjeta, 0 para efectivo y 3 para depósito bancario
                TN_EstadoPedidoId = 5, // Se asigna el estado como completado ya que este tipo de venta se hace de forma presencial
                TN_TransaccionId = transaccionId.ToString(),
                TF_Fecha = DateTime.Now,
                TN_CuponId = cupon.TN_Id,
                TN_Subtotal = carritoCompras.Sum(c => c.TN_Cantidad * c.Producto?.TN_Precio),
                TN_Impuesto = 0, // Asignar impuesto si es necesario
                TN_Descuento = descuento, // Asignar el descuento calculado
                TN_Total = totalFinal, // Asignar el total del carrito
                TB_Activo = true,
                TC_NumTarjeta = "00000000",
            };
            _context.TECO_P_Pedido.Add(pedido);
            await _context.SaveChangesAsync();

            var productosPedido = 
                await _context.TECO_P_CarritoCompras.Include(c => c.Producto)
                .Where(c => c.TN_UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var productoPedido in productosPedido.Select(producto => new TECO_P_DetallePedido
                     {
                         TN_PedidoId = pedido.TN_Id,
                         TN_ProductoId = producto.TN_ProductoId,
                         TN_Cantidad = producto.TN_Cantidad,
                         TN_PrecioUnitario = producto.Producto?.TN_Precio ?? 0,
                         TB_Activo = true
                     }))
            {
                _logger.LogCritical("Agregando producto al detalle del pedido: {ProductoPedido}", JsonSerializer.Serialize(productoPedido));
                _context.TECO_P_DetallePedido.Add(productoPedido);
            
                // Actualizar el stock del producto
                var productoEnInventario = await _context.TECO_A_Producto
                    .FirstOrDefaultAsync(p => p.TN_Id == productoPedido.TN_ProductoId);
                
                if (productoEnInventario != null)
                {
                    productoEnInventario.TN_Stock -= productoPedido.TN_Cantidad ?? 0;
                    _logger.LogCritical("Stock del producto ID {ProductoId} actualizado. Nuevo stock: {NuevoStock}",
                        productoPedido.TN_ProductoId, productoEnInventario.TN_Stock);
                }
            }
            // Eliminar el carrito de compras del usuario después de crear el pedido
            _context.TECO_P_CarritoCompras.RemoveRange(carritoCompras);
            await _context.SaveChangesAsync();
            TempData.Clear();
            TempData["success"] = JsonSerializer.Serialize(Alert.InfoAlert("Pedido creado correctamente."));
            HttpContext.Session.Remove("AppliedCouponId"); // Limpiar el cupón aplicado de la sesión
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error al crear el pedido: {Message}", e.Message);
            return RedirectToAction("Index");
        }
    }
}