using Microsoft.AspNetCore.Mvc;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class ProductosController : Controller
{
    /// <summary>
    /// Muestra la página principal de productos
    /// </summary>
    /// <returns>View principal que lista todos los productos disponibles (Solo se muestran los activos)</returns>
    public IActionResult Index()
    {
        return View();
    }
    
    /// <summary>
    /// Muestra todos los productos categorizados como componentes al usuario
    /// </summary>
    /// <returns> View con un listado de todos los componentes</returns>
    public IActionResult Componentes()
    {
        return View();
    }
    
    /// <summary>
    /// Muestra todos los productos categorizados como PC al usuario
    /// </summary>
    /// <returns> View con un listado de todos las computadoras ya armadas</returns>
    public IActionResult Pc()
    {
        return View();
    }
    
    /// <summary>
    /// Muestra todos los productos categorizados como periféricos al usuario
    /// </summary>
    /// <returns> View con un listado de todos los periféricos disponibles en la tienda</returns>
    public IActionResult Perifericos()
    {
        return View();
    }
    
}