using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class productosMasVendidosViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<productosMasVendidosViewComponent> _logger;


    public productosMasVendidosViewComponent(TecnoCoreDbContext context, ILogger<productosMasVendidosViewComponent> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var productosMasVendidos = await _context.TECO_P_DetallePedido
            .GroupBy(dp => dp.TN_ProductoId)
            .Select(g => new ProductosMasVendidosViewModel()
            {
                Producto = _context.TECO_A_Producto.SingleOrDefault(p => p.TN_Id == g.Key),
                TotalVentas = g.Sum(dp => dp.TN_Cantidad * dp.TN_PrecioUnitario)
            })
            .OrderByDescending(x => x.TotalVentas)
            .Take(5)
            .ToListAsync();

        if (productosMasVendidos.Count <= 0)
        {
            return View(new List<ProductosMasVendidosViewModel>());
        }
        
        return View(productosMasVendidos);
    }
}