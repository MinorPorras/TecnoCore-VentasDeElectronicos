using Microsoft.AspNetCore.Mvc;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class RolesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}