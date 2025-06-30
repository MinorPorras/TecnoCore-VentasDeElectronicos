using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.webcomponent;

public class CategoriesViewComponent: ViewComponent
{
    private readonly TecnoCoreDbContext _context;

    public CategoriesViewComponent(TecnoCoreDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categorias =  await _context.Categorias
            .Include(c => c.Subcategorias)
            .ToListAsync();
        return View(categorias);
    }
}