using Microsoft.AspNetCore.Mvc;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con los pedidos del sistema.
/// </summary>
public class PedidosController : Controller
{
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
    /// <param name="id">Identificador del usuario cuyos pedidos se quieren consultar.</param>
    /// <returns>La vista de pedidos filtrada por el usuario especificado.</returns>
    public IActionResult Pedidos_Usuario(int id)
    {
        return View();
    }

    public IActionResult Estados_Pedidos()
    {
        return View();
    }
}