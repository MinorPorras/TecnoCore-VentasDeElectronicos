using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class CarritoComprasViewComponent: ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CarritoComprasViewComponent> _logger;
    private readonly UserManager<TECO_A_Usuario> _userManager;

    public CarritoComprasViewComponent(TecnoCoreDbContext context, ILogger<CarritoComprasViewComponent> logger, UserManager<TECO_A_Usuario> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(string usuarioId)
    {
        //Cada usuario tiene un carrito de compras asociado a su ID de usuario.
        //Dentro de esta tabla cada fila representa un producto específico que el usuario ha agregado a su carrito de compras.
        //Por lo que se deben seleccionar varias filas que estén vinculadas al usuario.
        if (User.Identity.IsAuthenticated)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if (usuario != null)
                {
                    var productosCarrito = await _context.TECO_P_CarritoCompras
                        .Include(c => c.Producto)
                        .Where(c => c.TN_UsuarioId == usuario.Id)
                        .ToListAsync();

                    return View(productosCarrito);
                }
                else
                {
                    _logger.LogCritical("El usuario no existe o no está autenticado.");
                    TempData["Alert"] = Alert.ErrorAlert("El usuario no existe o no está autenticado.");
                    return View(new List<TECO_P_CarritoCompras>());
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error al obtener el carrito de compras: {EMessage}", e.Message);
                TempData["Alert"] = Alert.ErrorAlert("El usuario no existe o no está autenticado.");
                return View(new List<TECO_P_CarritoCompras>());
            }
        }
        else
        {
            _logger.LogCritical("El usuario no está autenticado.");
            TempData["Alert"] = Alert.ErrorAlert("El usuario no existe o no está autenticado.");
            return View(new List<TECO_P_CarritoCompras>());
        }
    }
}