using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con las ventas del sistema.
/// </summary>
public class VentasController : Controller
{
    private readonly TecnoCoreDbContext _context = new();

    /// <summary>
    /// Muestra la vista principal de ventas.
    /// </summary>
    /// <returns>La vista Index de ventas.</returns>
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Carro_Compras(List<Productos> listaCompras)
    {
        return View();
    }

    public async Task<IActionResult> GetCartItems(int usuarioId)
    {
        var cartItems = await _context.CarritoCompras
            .Where(c => c.UsuarioId == usuarioId)
            .ToListAsync();
        if (!cartItems.Any()) return NotFound("No hay productos en el carrito de compras.");
        return Json(cartItems);
    }

    public async Task<IActionResult> RemoveFromCart(int usuarioId, int productoId)
    {
        try
        {
            var existingProducto = await _context.CarritoCompras
                .Where(c => c.UsuarioId == usuarioId && c.ProductoId == productoId)
                .FirstOrDefaultAsync();

            if (existingProducto == null) return NotFound("Producto no encontrado en el carrito de compras.");
            _context.CarritoCompras.Remove(existingProducto);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Json(new { success = false });
        }
    }


    public IActionResult ConfirmarCompra(List<Productos> listaCompras)
    {
        // Aquí se puede implementar la lógica para confirmar la compra
        // Por ejemplo, guardar la compra en una base de datos o procesar el pago

        // Redirigir a una vista de confirmación o al índice de ventas
        return RedirectToAction("Index");
    }
}