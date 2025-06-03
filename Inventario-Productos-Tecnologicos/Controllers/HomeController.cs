using System.Diagnostics;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TecnoCoreDbContext _dbContext = new TecnoCoreDbContext();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login(string email, string contrasena)
    {
        var usuario = _dbContext.Usuarios.FirstOrDefault(u =>
            u.Email == email && u.Contrasena == contrasena && u.RolNavigation != null &&
            u.RolNavigation.Name == "Admin");
        if (usuario == null)
        {
            ViewBag.Error = "Usuario o contraseña incorrectos";
        }

        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}