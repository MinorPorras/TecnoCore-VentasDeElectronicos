using Microsoft.AspNetCore.Mvc;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class PerifericosController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}