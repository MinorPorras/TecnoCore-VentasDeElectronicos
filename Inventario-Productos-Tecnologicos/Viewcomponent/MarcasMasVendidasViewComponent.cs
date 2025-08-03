using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class MarcasMasVendidasViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<MarcasMasVendidasViewComponent> _logger;


    public MarcasMasVendidasViewComponent(TecnoCoreDbContext context, ILogger<MarcasMasVendidasViewComponent> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var marcas = await _context.TECO_P_DetallePedido
            .GroupBy(dp => dp.Producto.TN_MarcaId)
            .Select(g => new MarcasMasVendidas()
            {
                Marca = _context.TECO_M_Marca.SingleOrDefault(m => m.TN_Id == g.Key),
                TotalVentas = g.Sum(dp => dp.TN_Cantidad * dp.TN_PrecioUnitario)
            })
            .OrderByDescending(x => x.TotalVentas)
            .Take(3)
            .ToListAsync();

        return View(marcas.Count == 0 ? [] : marcas);
    }
}