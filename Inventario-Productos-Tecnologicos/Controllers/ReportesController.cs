using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Inventario_Productos_Tecnologicos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class ReportesController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<ReportesController> _logger;

    public ReportesController(TecnoCoreDbContext context, ILogger<ReportesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult ReporteVentas()
    {
        // Se inicializa el ViewModel con fechas por defecto y una lista vacía
        var viewModel = new ReporteVentasViewModel
        {
            fechaInicio = new DateTime(2000, 1, 1), // Una fecha antigua por defecto
            fechaFin = DateTime.Now,                // La fecha actual por defecto
            ListPedidos = []
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReporteVentas(DateTime fechaInicio, DateTime fechaFin)
    {
        
        _logger.LogCritical("Fecha inicio: " + fechaInicio);
        _logger.LogCritical("Fecha Fin: " + fechaFin);
        // Consulta LINQ con Eager Loading para obtener todos los datos relacionados
        var pedidos = await _context.TECO_P_Pedido
            .Include(p => p.Usuario)
            .Include(p => p.MetodoPago)
            .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.Producto)
                    .ThenInclude(prod => prod.Marca)
            .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.Producto)
                    .ThenInclude(prod => prod.Subcategoria)
            .Where(p => p.TF_Fecha >= fechaInicio.Date && p.TF_Fecha <= fechaFin.Date.AddDays(1).AddSeconds(-1))
            .ToListAsync();

        // Lógica para calcular totales y encontrar los más vendidos
        var totalVentas = pedidos.Sum(p => p.TN_Total) ?? 0;
        var totalDescuentos = pedidos.Sum(p => p.TN_Descuento) ?? 0;

        // Obtener una lista plana de todos los detalles de los pedidos
        var todosLosDetalles = pedidos.SelectMany(p => p.DetallePedidos).ToList();

        // Encontrar el nombre del producto más vendido
        var productoMejorVendidoNombre = todosLosDetalles
            .GroupBy(d => d.Producto)
            .OrderByDescending(g => g.Sum(d => d.TN_Cantidad))
            .Select(g => g.Key.TC_Nombre) // Seleccionamos solo el nombre
            .FirstOrDefault();

        // Encontrar el nombre de la marca más vendida
        var marcaMejorVendidaNombre = todosLosDetalles
            .Where(d => d.Producto?.Marca != null)
            .GroupBy(d => d.Producto.Marca)
            .OrderByDescending(g => g.Sum(d => d.TN_Cantidad))
            .Select(g => g.Key.TC_Nombre) // Seleccionamos solo el nombre de la marca
            .FirstOrDefault();
    
        // Encontrar el nombre de la subcategoría más vendida
        var subcategoriaMejorVendidaNombre = todosLosDetalles
            .Where(d => d.Producto?.Subcategoria != null)
            .GroupBy(d => d.Producto.Subcategoria)
            .OrderByDescending(g => g.Sum(d => d.TN_Cantidad))
            .Select(g => g.Key.TC_Nombre) // Seleccionamos solo el nombre de la subcategoría
            .FirstOrDefault();
        
        _logger.LogCritical("Cont pedidos" + pedidos.Count);
        _logger.LogCritical("totalVentas" + totalVentas);
        _logger.LogCritical("totalDescuentos" + totalDescuentos);
        _logger.LogCritical("productoMejorVendidoNombre" + productoMejorVendidoNombre);
        _logger.LogCritical("totalVentas" + marcaMejorVendidaNombre);
        _logger.LogCritical("totalVentas" + subcategoriaMejorVendidaNombre);




        // Crear y llenar el ViewModel con los resultados
        var viewModel = new ReporteVentasViewModel
        {
            fechaInicio = fechaInicio,
            fechaFin = fechaFin,
            total = totalVentas,
            descuento = totalDescuentos,
            productoMejorVendido = productoMejorVendidoNombre,
            marcaMejorVendida = marcaMejorVendidaNombre,
            subcategoriaMejorVendida = subcategoriaMejorVendidaNombre,
            ListPedidos = pedidos
        };

        return View(viewModel);
    }
}