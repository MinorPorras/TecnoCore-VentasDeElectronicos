using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Models;

namespace Inventario_Productos_Tecnologicos.webcomponent;

public class ProductCardListViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;

    public ProductCardListViewComponent(TecnoCoreDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(string title, int subcategoryId = 0, int categoryId = 0,
        int lenght = 9)
    {
        ViewBag.ListLenght = lenght;
        ViewBag.ListTitle = title;
        if ( subcategoryId > 0)
        {
            Console.WriteLine("Subcategoría: " + subcategoryId);
            var productosSubcategoria = await _context.Productos
                .Where(p => p.SubcategoriaId == subcategoryId && p.Activo)
                .Include(p => p.Subcategoria)
                .Take(lenght)
                .ToListAsync();
            return View(productosSubcategoria);
        }
        else if ( categoryId > 0)
        {
            var productosCategoria = await _context.Productos
                .Where(p => p.Subcategoria!.CategoriaId == categoryId && p.Activo)
                .Include(p => p.Subcategoria)
                .ThenInclude(s => s.Categoria)
                .Take(lenght)
                .ToListAsync();
            return View(productosCategoria);
        }

        var products = await _context.Productos
            .Where(p => p.Activo)
            .Include(p => p.Subcategoria)
            .ThenInclude(s => s.Categoria)
            .Take(lenght)
            .ToListAsync();
        return View(products);
    }
}