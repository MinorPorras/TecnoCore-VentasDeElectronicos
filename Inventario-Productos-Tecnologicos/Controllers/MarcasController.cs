using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Data;

namespace Inventario_Productos_Tecnologicos.Controllers
{
    public class MarcasController : Controller
    {
        private readonly TecnoCoreDbContext _context;

        public MarcasController(TecnoCoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var marcas = await _context.Marcas.ToListAsync();
            return View(marcas); // Esto llena el Model con una lista válida
        }
    }
}
