using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con las ventas del sistema.
/// </summary>
public class VentasController : Controller
{
    /// <summary>
    /// Muestra la vista principal de ventas.
    /// </summary>
    /// <returns>La vista Index de ventas.</returns>
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Carro_Compras(List<Productos> listaCompras)
    {
        return View();
    }
}