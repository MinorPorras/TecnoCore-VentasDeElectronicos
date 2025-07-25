using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class CategoriaMasVendidaViewComponent: ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<CategoriaMasVendidaViewComponent> _logger;

    public CategoriaMasVendidaViewComponent(TecnoCoreDbContext context, ILogger<CategoriaMasVendidaViewComponent> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categoriasMasVendidas = await _context.TECO_P_DetallePedido
            .GroupBy(dp => dp.Producto.TN_SubcategoriaId)
            .Select(g => new CategoriaMasVendidaViewModel()
            {
                subCategoria = _context.TECO_M_Subcategoria.FirstOrDefault(s => s.TN_Id == g.Key),
                TotalVentas = g.Sum(dp => dp.TN_Cantidad * dp.TN_PrecioUnitario)
            })
            .OrderByDescending(x => x.TotalVentas)
            .Take(5)
            .ToListAsync();
        
        return View(categoriasMasVendidas);
    }
}