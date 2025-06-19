using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con los usuarios del sistema.
/// </summary>
public class UsuariosController : Controller
{
    private readonly TecnoCoreDbContext _context;

    public UsuariosController(TecnoCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Muestra la vista principal de usuarios.
    /// </summary>
    /// <returns>La vista Index de usuarios.</returns>
    public async Task<IActionResult> Index()
    {
        // Cargar los roles activos para el dropdown
        ViewBag.Roles = await _context.Roles.Where(r => r.Activo == true).ToListAsync();
        ViewBag.Rol = "all"; // Establecer el valor por defecto

        var usuarios = await _context.Usuarios
            .Include(u => u.RolNavigation)
            .ToListAsync();
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

    public async Task<IActionResult> Search(string searchElement, string activeFilter, string rol)
    {
        ViewBag.SearchString = searchElement;
        ViewBag.ActiveFilter = activeFilter;
        ViewBag.Rol = rol;

        // Obtener todos los roles para el dropdown
        ViewBag.Roles = await _context.Roles.Where(r => r.Activo == true).ToListAsync();

        IQueryable<Usuarios> query = _context.Usuarios
            .Include(u => u.RolNavigation);

        // Aplicar filtro de búsqueda si existe
        if (!string.IsNullOrEmpty(searchElement))
            query = query.Where(u => u.Nombre.Contains(searchElement)
                                     || u.Email.Contains(searchElement)
                                     || u.Id.ToString().Contains(searchElement));

        // Aplicar filtro de estado si no es "all"
        if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
        {
            var isActive = activeFilter == "true";
            query = query.Where(u => u.Activo == isActive);
        }

        // Aplicar filtro de rol si no es "all"
        if (rol != "all" && !string.IsNullOrEmpty(rol))
            query = query.Where(u => u.RolNavigation != null && u.RolNavigation.ToString() == rol);

        var usuarios = await query.ToListAsync();
        return View("Index", usuarios);
    }

    public IActionResult Create()
    {
        // Cargar roles para el dropdown
        ViewBag.Roles = _context.Roles.Where(r => r.Activo == true).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Email", "Nombre", "Apellidos", "Contrasena", "Telefono")]
        Usuarios usuario, int rol, bool active)
    {
        if (!ModelState.IsValid)
        {
            // Recargar los roles para el dropdown en caso de error
            ViewBag.Roles = await _context.Roles.Where(r => r.Activo == true).ToListAsync();
            return View(usuario);
        }

        try
        {
            // Asignar el rol por defecto si no se especifica
            if (rol != 0)
            {
                usuario.Rol = rol;
            }
            else
            {
                // Si no se especifica un rol, asignar el rol por defecto "Cliente"
                var defaultRol = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Cliente");
                if (defaultRol != null) usuario.Rol = defaultRol.Id;
            }

            usuario.Activo = active;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Recargar los roles para el dropdown en caso de error
            ViewBag.Roles = await _context.Roles.Where(r => r.Activo == true).ToListAsync();
            return View(usuario);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();
        try
        {
            usuario.Activo = !usuario.Activo;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(400, new { message = "No se pudo eliminar el elemento en la base de datos" });
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        // Cargar roles para el dropdown
        ViewBag.Roles = await _context.Roles.Where(r => r.Activo == true).ToListAsync();
        return View(usuario);
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] Usuarios usuario)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(new { message = "Datos inválidos enviados al servidor." });

            var existingUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);
            if (existingUsuario == null) return NotFound();

            // Actualizar los campos del usuario
            existingUsuario.Email = usuario.Email;
            existingUsuario.Nombre = usuario.Nombre;
            existingUsuario.Apellidos = usuario.Apellidos;
            existingUsuario.Contrasena = usuario.Contrasena;
            existingUsuario.Telefono = usuario.Telefono;
            existingUsuario.Activo = usuario.Activo;
            existingUsuario.Rol = usuario.Rol;

            _context.Usuarios.Update(existingUsuario);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine($"Error al actualizar el usuario: {e.Message}");
            return StatusCode(500, new { message = "No se pudo guardar los cambios en la base de datos" });
        }
    }
}