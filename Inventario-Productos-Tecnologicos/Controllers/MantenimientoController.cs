using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Inventario_Productos_Tecnologicos.Controllers;

[Authorize]
public class MantenimientoController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}