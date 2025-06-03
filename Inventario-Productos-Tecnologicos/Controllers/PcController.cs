using Microsoft.AspNetCore.Mvc;

namespace Inventario_Productos_Tecnologicos.Data;

public class PcController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}