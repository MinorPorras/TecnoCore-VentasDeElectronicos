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
    /// <param name="contrasena">La contraseña del usuario.</param>
    /// <returns>Una redirección a la acción correspondiente según el resultado del inicio de sesión.</returns>
    public RedirectToActionResult Login(string email, string contrasena)
    {
        // Descomentar y modificar el siguiente código para implementar la lógica de inicio de sesión:
        var usuario = _context.Usuarios.Include(usuario => usuario.RolNavigation).FirstOrDefault(u =>
            u.Email == email && u.Contrasena == contrasena && u.RolNavigation != null &&
            u.RolNavigation.Name == "Admin");
        if (usuario == null) ViewBag.Error = "Usuario o contraseña incorrectos";
        // Guardar información relevante en sesión
        if (usuario is { RolNavigation: not null })
        {
            HttpContext.Session.SetInt32("UserId", usuario.Id);
            HttpContext.Session.SetString("UserRole", usuario.RolNavigation.Name);
            HttpContext.Session.SetString("UserName", usuario.Nombre);
            if (usuario.RolNavigation.Name == "Cliente")
                return RedirectToAction("Mantenimiento", "Home");
            else
                return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Mantenimiento");
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