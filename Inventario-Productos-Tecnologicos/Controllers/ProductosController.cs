using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class ProductosController : Controller
{
    
    private readonly TecnoCoreDbContext _context;

    public ProductosController(TecnoCoreDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Muestra la página principal de productos
    /// </summary>
    /// <returns>View principal que lista todos los productos disponibles (Solo se muestran los activos)</returns>
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> PorCategoria(int id)
    {
        var categoria = await _context.Categorias.Include(c => c.Subcategorias).FirstOrDefaultAsync(c => c.Id == id);
        return View(categoria);
    }
    
    [HttpPost]
    public  async Task<IActionResult> PorSubcategoria(int id)
    {
        var subcategoria = await _context.Subcategorias.FirstOrDefaultAsync(s => s.Id == id);
        return View(subcategoria);
    }
}