using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;


namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class CarruselNovedadesViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<CarruselNovedadesViewComponent> _logger;

    public CarruselNovedadesViewComponent(TecnoCoreDbContext context, IWebHostEnvironment webHostEnvironment,
        ILogger<CarruselNovedadesViewComponent> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task<ViewViewComponentResult> InvokeAsync()
    {
        var productos = await _context.TECO_A_Producto
            .Where(p => p.TB_Novedad)
            .ToListAsync();
        ViewBag.Cant = productos.Count;
        foreach (var prod in productos) prod.TC_Imagen = GetImagePath(prod.TC_Imagen);

        return View(productos);
    }

    private string GetImagePath(string? imagePath)
    {
        const string defaultImageUrl = "/img/productos/default-image.jpg";
        if (string.IsNullOrEmpty(imagePath)) return defaultImageUrl;
        // Convertir la ruta relativa de la BD a ruta física
        var rutaFisica = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
        // Verificar si el archivo existe y devolver la ruta URL relativa original
        if (File.Exists(rutaFisica)) return imagePath;
        return defaultImageUrl;
    }
}