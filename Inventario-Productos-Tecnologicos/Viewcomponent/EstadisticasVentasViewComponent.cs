using System.Globalization;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class EstadisticasVentasViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<EstadisticasVentasViewComponent> _logger;


    public EstadisticasVentasViewComponent(TecnoCoreDbContext context, ILogger<EstadisticasVentasViewComponent> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        //Se crea la instancia del modelo
        var viewModel = new EstadisticasVentasViewModel();
        
        //Se obtiene el total de ventas del sistema
        viewModel.TotalVentas = await _context.TECO_P_Pedido.SumAsync(p => p.TN_Total);
        
        //Se obtiene el año con mejor ventas
        var bestYear = await _context.TECO_P_Pedido
            .Where(p => p.TF_Fecha != null)
            .GroupBy(p => p.TF_Fecha.Value.Year)
            .Select(g => new
            {
                year = g.Key,
                total = g.Sum(p => p.TN_Total)
            })
            .OrderByDescending(x => x.total)
            .FirstOrDefaultAsync();

        if (bestYear != null)
        {
            viewModel.MejorAñoVentas = bestYear.year.ToString();
            viewModel.TotalMejorAñoVentas = (decimal)bestYear.total;
        }

        var bestMonth = await _context.TECO_P_Pedido
            .Where(p => p.TF_Fecha != null)
            .GroupBy(p => new { year = p.TF_Fecha.Value.Year, month = p.TF_Fecha.Value.Month })
            .Select(g => new
            {
                Year = g.Key.year,
                Month = g.Key.month,
                Total = g.Sum(p => p.TN_Total ?? 0)
            })
            .OrderByDescending(x => x.Total)
            .FirstOrDefaultAsync();

        var formatoEsp = new CultureInfo("es-ES");

        if (bestMonth != null)
        {
            viewModel.MejorMesVentas = formatoEsp.DateTimeFormat.GetMonthName(bestMonth.Month);
            viewModel.TotalMejorMesVentas = bestMonth.Total;
        }

        var bestDay = await _context.TECO_P_Pedido
            .Where(p => p.TF_Fecha != null)
            .GroupBy(p => p.TF_Fecha.Value.Date)
            .Select(g => new
            {
                Fecha = g.Key,
                Total = g.Sum(p => p.TN_Total ?? 0)
            })
            .OrderByDescending(x => x.Total)
            .FirstOrDefaultAsync();

        if (bestDay != null)
        {
            viewModel.MejorDiaVentas = bestDay.Fecha.ToString("dd 'de' MMMM 'de' yyyy", formatoEsp);
            viewModel.TotalMejorDiaVentas = bestDay.Total;
        }
        
        
        return View(viewModel);
    }
}