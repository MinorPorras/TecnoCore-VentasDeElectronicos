using System.Diagnostics;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja la lógica principal de la aplicación, incluyendo el inicio de sesión y la navegación.
/// </summary>
public class HomeController : Controller
{
    private readonly TecnoCoreDbContext _context;

    public HomeController(TecnoCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Muestra la vista principal del índice.
    /// </summary>
    /// <returns>La vista Index.</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Muestra la vista de mantenimiento.
    /// </summary>
    /// <returns>La vista Mantenimiento.</returns>
    public ViewResult Mantenimiento()
    {
        return View();
    }

    /// <summary>
    /// Muestra la vista de política de privacidad.
    /// </summary>
    /// <returns>La vista Privacy.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Maneja los errores y muestra la vista de error.
    /// </summary>
    /// <returns>La vista Error con los detalles del error.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}