using System.Linq; // Necesario para LINQ, como .FirstOrDefault() o .Any()
using Microsoft.AspNetCore.Identity; // Necesario para UserManager y SignInManager
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models; // Tu modelo Usuarios, Provincia, Canton, Direccion
using Inventario_Productos_Tecnologicos.Models.ViewModels; // Tu RegisterViewModel
using Inventario_Productos_Tecnologicos.Data; // Tu DbContext
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // Para ToListAsync()

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con los usuarios del sistema.
/// </summary>
public class UsuariosController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly UserManager<Usuarios> _userManager;
    private readonly SignInManager<Usuarios> _signInManager;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(TecnoCoreDbContext context,
        UserManager<Usuarios> userManager, SignInManager<Usuarios> signInManager,
        ILogger<UsuariosController> logger)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<ViewResult> Register()
    {
        var model = new RegisterViewModel();
        await RellenarProvinciasCantones(model);
        return View(model);
    }

    private async Task RellenarProvinciasCantones(RegisterViewModel model)
    {
        model.Provincias = await _context.Provincias
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nombre
            }).ToListAsync();

        if (model.SelectedProvinciaId == 0)
            model.Cantones.Add(new SelectListItem { Value = "", Text = "--Seleccione primero una provincia--" });
        else
            model.Cantones = await _context.Cantones
                .Where(p => p.ProvinciaId == model.SelectedProvinciaId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }).ToListAsync();
    }

    [HttpGet] // Especifica que es un método HTTP GET
    [Route("Usuarios/GetCantonesByProvincia/{provinciaId}")] // Define la ruta para este método
    public async Task<IActionResult> GetCantonesByProvincia(int provinciaId)
    {
        // Verifica si el ID de provincia es válido (opcional, pero buena práctica)
        if (provinciaId <= 0) return BadRequest("ID de provincia inválido.");

        // Obtener los cantones de la base de datos para la provincia dada
        var cantones = await _context.Cantones
            .Where(c => c.ProvinciaId == provinciaId)
            .Select(c => new
            {
                id = c.Id,
                nombre = c.Nombre
            }) // Selecciona solo las propiedades necesarias para el JSON
            .ToListAsync();

        // Devolver la lista de cantones como JSON
        return Json(cantones);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
            //Se inicia un proceso de transacción, este tipo de procesos ejecutan
            //las acciones en la DB pero si una falla las demás se descartan con ella
            //En este caso se guarda tod*o no se guarda nada
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new Usuarios
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Nombre = model.Nombre,
                        Apellidos = model.Apellidos, // El campo único para apellidos
                        PhoneNumber = model.PhoneNumber,
                        EmailConfirmed = true, // Podrías tener un proceso de confirmación por email
                        Activo = true // Asumimos que el usuario está activo al registrarse
                    };

                    //Se crea el usuario
                    var result = await _userManager.CreateAsync(user, model.Password);

                    //EN caso de que se cree se pasa a asignarle un rol por defecto
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Usuario Creado exitosamente");

                        //Si no tienje un rol asignado se le agrega el rol de Cliente
                        if (!await _userManager.IsInRoleAsync(user, "Cliente"))
                        {
                            await _userManager.AddToRoleAsync(user, "Cliente");
                            _logger.LogInformation("Usuario asignado al rol cliente exitosamente");
                        }
                        else
                        {
                            _logger.LogWarning($"El usuario {user.UserName} ya tiene un rol asignado");
                        }

                        //Se obtiene la provincia y el cantón seleccionados
                        var provincia =
                            await _context.Provincias.FirstOrDefaultAsync(p => p.Id == model.SelectedProvinciaId);
                        var canton =
                            await _context.Cantones.FirstOrDefaultAsync(p => p.Id == model.SelectedCantonId);

                        if (provincia == null && canton == null)
                        {
                            //Si no se ecneuntra nada se hace un rollback para quitar los cambios
                            ModelState.AddModelError(string.Empty, "Provincia o cantón seleccionados no son válidos");
                            await transaction.RollbackAsync();
                            await RellenarProvinciasCantones(model);
                            return View(model);
                        }

                        //Creación de la dirección del usuario
                        var direccion = new Direcciones
                        {
                            Direccion = model.DireccionExacta,
                            CodigoPostal = model.CodigoPostal,
                            CantonId = canton?.Id,
                            UsuarioId = user.Id,
                            Activo = true
                        };
                        //Se agrega en la abse de datos la dirección del usuario
                        _context.Direcciones.Add(direccion);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Dirección almacenada correctamente");

                        //Se hace el commit de la transacción para guardar los cambios realizados en la DB
                        await transaction.CommitAsync();

                        await _signInManager.SignInAsync(user, false);
                        _logger.LogInformation("Usuario registrado y ha iniciado sesión");

                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

        // Si el ModelState no es válido o hubo errores, re-renderizar la vista
        await RellenarProvinciasCantones(model);
        return View(model);
    }

    public ViewResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Datos de login invalidos");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
        if (result.Succeeded)
        {
            if (User.IsInRole("Administrador"))
                return RedirectToAction("Mantenimiento", "Home");
            else
                return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    /*[HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Intento de login con datos invalidos");
            foreach (var error in ModelState.Values)
            {
                TempData["LoginError"] = error.Errors[0].ErrorMessage;
                break;
            }
            TempData["LoginError"] = null;
        }
        return RedirectToAction(nameof(Index));
    }*/

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index), "Home");
    }


    /*/// <summary>
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
            query = query.Where(u => u.RolNavigation != null && u.Rol == Convert.ToInt32(rol));

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
    }*/
    public IActionResult Informacion_personal()
    {
        return View();
    }
}