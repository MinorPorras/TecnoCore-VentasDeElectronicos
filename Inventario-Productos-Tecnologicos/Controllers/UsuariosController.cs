using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con los usuarios del sistema.
/// </summary>
public class UsuariosController : Controller
{
    private readonly TecnoCoreDbContext _context = new TecnoCoreDbContext();

    /// <summary>
    /// Muestra la vista principal de usuarios.
    /// </summary>
    /// <returns>La vista Index de usuarios.</returns>
    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        return View(usuarios);
    }

    /// <summary>
    /// Muestra la información personal de un usuario específico.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>La vista con la información personal del usuario.</returns>
    public async Task<IActionResult> Informacion_Personal(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        return View(usuario);
    }

    ///<summary>
    /// Muestra la lista de deseos de un usuario específico.
    /// </summary>
    /// <param name= "id"/>El identificador único del usuario.
    /// <returns>La vista con la lista de deseos del usuario.</returns>
    public async Task<IActionResult> Lista_Deseos(int id)
    {
        var listaDeseos = await _context.ListaDeseos.Where(ld => ld.UsuarioId == id).ToListAsync();
        return View(listaDeseos);
    }

    public IActionResult Search()
    {
        throw new NotImplementedException();
    }
}