using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class InfoPedidosViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<InfoPedidosViewComponent> _logger;


    public InfoPedidosViewComponent(TecnoCoreDbContext context, ILogger<InfoPedidosViewComponent> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var recuentoEstados = await _context.TECO_P_Pedido
            .Where(p => p.TN_EstadoPedidoId != null)
            .GroupBy(p => p.TN_EstadoPedidoId)
            .Select(g => new { EstadoId = g.Key, Cantidad = g.Count() })
            .ToListAsync();
        var info = new InfoPedidosViewModel();

        foreach (var item in  recuentoEstados)
        {
            switch (item.EstadoId)
            {
                case 1:
                    info.Pendiente = item.Cantidad;
                    break;
                case 2:
                    info.Confirmado = item.Cantidad;
                    break;
                case 3:
                    info.EnProceso = item.Cantidad;
                    break;
                case 4:
                    info.Enviado = item.Cantidad;
                    break;
                case 5:
                    info.Entregado = item.Cantidad;
                    break;
                case 6:
                    info.Cancelado = item.Cantidad;
                    break;
            }
        }

        return View(info);
    }
}