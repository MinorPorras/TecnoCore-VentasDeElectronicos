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
    /// Maneja el inicio de sesión del usuario y redirecciona según el rol.
    /// </summary>
    /// <param name="email">El correo electrónico del usuario.</param>
    /// <param name="password">La contraseña del usuario.</param>
    /// <returns>Una redirección a la acción correspondiente según el resultado del inicio de sesión.</returns>
    [HttpPost]
    public RedirectToActionResult Login(string email, string password)
    {
        // Descomentar y modificar el siguiente código para implementar la lógica de inicio de sesión:s
        var usuario = _context.Usuarios
            .Include(u => u.RolNavigation)
            .FirstOrDefault(u => u.Email == email && u.Contrasena == password);
        if (usuario == null)
        {
            TempData["Error"] = "Usuario o contraseña incorrectos";
            return RedirectToAction("Index");
        }

        Console.WriteLine("Email: " + usuario.Email);
        Console.WriteLine("Contraseña: " + usuario.Contrasena);
        // Guardar información relevante en sesión
        HttpContext.Session.SetInt32("UserId", usuario.Id);
        HttpContext.Session.SetString("UserRole", usuario.RolNavigation.Name);
        HttpContext.Session.SetString("UserName", usuario.Nombre);
        Console.WriteLine("UserRole:" + HttpContext.Session.GetString("UserRole"));
        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        return RedirectToAction(usuario.RolNavigation.Name == "Administrador"
            ? "Mantenimiento"
            : "Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
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