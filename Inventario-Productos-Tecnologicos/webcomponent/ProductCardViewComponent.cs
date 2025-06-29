using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;


namespace Inventario_Productos_Tecnologicos.webcomponent;

public class ProductCardViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductCardViewComponent(TecnoCoreDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId)
    {
        var product = await _context.Productos
            .Include(p => p.Subcategoria)
            .Include(p => p.Subcategoria!.Categoria)
            .FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return Content("Producto no encontrado");
        // Verificar y asignar la ruta de la imagen
        product.Imagen = GetImagePath(product.Imagen);
        return View(product);
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