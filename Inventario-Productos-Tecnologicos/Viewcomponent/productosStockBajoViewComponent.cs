using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;


namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class ProductosStockBajoViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<ProductosStockBajoViewComponent> _logger;
    
    public ProductosStockBajoViewComponent(TecnoCoreDbContext context, ILogger<ProductosStockBajoViewComponent> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var productosStockBajo = await _context.TECO_A_Producto
            .OrderBy(p => p.TN_Stock)
            .Take(5)
            .ToListAsync();
        _logger.LogInformation("Productos con estock bajo cargados");

        return View(productosStockBajo.Count <= 0 ? [] : productosStockBajo);
    }
}