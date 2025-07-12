using Microsoft.AspNetCore.Identity; // Necesario para UserManager y SignInManager
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models; // Tu modelo Usuarios, Provincia, Canton, Direccion
using Inventario_Productos_Tecnologicos.Models.ViewModels; // Tu RegisterViewModel
using Inventario_Productos_Tecnologicos.Data; // Tu DbContext
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con los usuarios del sistema.
/// </summary>
public class AccountController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly UserManager<TECO_A_Usuario> _userManager;
    private readonly SignInManager<TECO_A_Usuario> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(TecnoCoreDbContext context,
        UserManager<TECO_A_Usuario> userManager, SignInManager<TECO_A_Usuario> signInManager,
        ILogger<AccountController> logger)
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
        var model = new RegisterViewModel
        {
            SelectedCantonId = 0,
            SelectedProvinciaId = 0
        };
        await RellenarProvinciasCantones(model);
        return View(model);
    }

    private async Task RellenarProvinciasCantones(RegisterViewModel model)
    {
        model.Provincias = await _context.TECO_M_Provincia
            .Select(p => new SelectListItem
            {
                Value = p.TN_Id.ToString(),
                Text = p.TC_Nombre
            }).ToListAsync();

        if (model.SelectedProvinciaId == 0)
            model.Cantones.Add(new SelectListItem { Value = "", Text = "--Seleccione primero una provincia--" });
        else
            model.Cantones = await _context.TECO_M_Canton
                .Where(p => p.TN_ProvinciaId == model.SelectedProvinciaId)
                .Select(c => new SelectListItem
                {
                    Value = c.TN_Id.ToString(),
                    Text = c.TC_Nombre
                }).ToListAsync();
    }

    [HttpGet] // Especifica que es un método HTTP GET
    [Route("Usuarios/GetCantonesByProvincia/{provinciaId}")] // Define la ruta para este método
    public async Task<IActionResult> GetCantonesByProvincia(int provinciaId)
    {
        // Verifica si el ID de provincia es válido (opcional, pero buena práctica)
        if (provinciaId <= 0) return BadRequest("ID de provincia inválido.");

        // Obtener los cantones de la base de datos para la provincia dada
        var cantones = await _context.TECO_M_Canton
            .Where(c => c.TN_ProvinciaId == provinciaId)
            .Select(c => new
            {
                id = c.TN_Id,
                nombre = c.TC_Nombre
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
                    var user = new TECO_A_Usuario
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        TC_Nombre = model.Nombre,
                        TC_Apellidos = model.Apellidos, // El campo único para apellidos
                        PhoneNumber = model.PhoneNumber,
                        EmailConfirmed = true, // Podrías tener un proceso de confirmación por email
                        TB_Activo = true // Asumimos que el usuario está activo al registrarse
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
                            await _context.TECO_M_Provincia.FirstOrDefaultAsync(p =>
                                p.TN_Id == model.SelectedProvinciaId);
                        var canton =
                            await _context.TECO_M_Canton.FirstOrDefaultAsync(p => p.TN_Id == model.SelectedCantonId);

                        if (provincia == null && canton == null)
                        {
                            //Si no se ecneuntra nada se hace un rollback para quitar los cambios
                            ModelState.AddModelError(string.Empty, "Provincia o cantón seleccionados no son válidos");
                            await transaction.RollbackAsync();
                            await RellenarProvinciasCantones(model);
                            return View(model);
                        }

                        //Creación de la dirección del usuario
                        var direccion = new TECO_A_Direccion
                        {
                            TC_Direccion = model.DireccionExacta,
                            TC_CodigoPostal = model.CodigoPostal,
                            TN_CantonId = canton.TN_Id,
                            TN_UsuarioId = user.Id,
                            TB_Activo = true
                        };
                        //Se agrega en la abse de datos la dirección del usuario
                        _context.TECO_A_Direccion.Add(direccion);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Dirección almacenada correctamente");

                        //Se hace el commit de la transacción para guardar los cambios realizados en la DB
                        await transaction.CommitAsync();

                        await _signInManager.SignInAsync(user, false);
                        _logger.LogInformation("Usuario registrado y ha iniciado sesión");

                        var alert = new Alert { Message = "Registro exitoso", Type = "success" };
                        TempData["success"] = System.Text.Json.JsonSerializer.Serialize(alert);

                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

                    var errorAlert = new Alert { Message = "Error al registrar usuario", Type = "error" };
                    TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(errorAlert);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error durante el registro de usuario");
                    var errorAlert = new Alert { Message = "Error interno del servidor", Type = "error" };
                    TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(errorAlert);
                    await transaction.RollbackAsync();
                }
            }

        // Si el ModelState no es válido o hubo errores, re-renderizar la vista
        await RellenarProvinciasCantones(model);
        return View(model);
    }

    public async Task<IActionResult> Informacion_personal(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();
        var direccion = await _context.TECO_A_Direccion.Where(d => d.TN_UsuarioId == usuario.Id)
            .Include(d => d.Canton).FirstOrDefaultAsync();
        if (direccion == null) return NotFound();
        try
        {
            if (direccion.Canton != null && usuario is
                    { Email: not null, UserName: not null, PhoneNumber: not null })
            {
                var model = new RegisterViewModel
                {
                    UserName = usuario.UserName,
                    Email = usuario.Email,
                    Nombre = usuario.TC_Nombre,
                    Apellidos = usuario.TC_Apellidos,
                    PhoneNumber = usuario.PhoneNumber,
                    DireccionExacta = direccion.TC_Direccion,
                    CodigoPostal = direccion.TC_CodigoPostal,
                    SelectedCantonId = direccion.TN_CantonId,
                    SelectedProvinciaId = direccion.Canton.TN_ProvinciaId
                };
                var provincia = await _context.TECO_M_Provincia.Where(p => p.TN_Id == model.SelectedProvinciaId)
                    .FirstOrDefaultAsync();
                var canton = await _context.TECO_M_Canton.Where(c => c.TN_Id == model.SelectedCantonId)
                    .FirstOrDefaultAsync();
                ViewBag.ProvinciaName = provincia?.TC_Nombre;
                ViewBag.CantonName = canton?.TC_Nombre;
                return View(model);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return RedirectToAction("Index", "Home");
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
            var alert = new Alert { Message = "Credenciales inválidas", Type = "error" };
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(alert);
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
        if (result.Succeeded)
        {
            var alert = new Alert { Message = "Inicio de sesión exitoso", Type = "success" };
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(alert);

            if (User.IsInRole("Administrador"))
                return RedirectToAction("Mantenimiento", "Home");
            else
                return RedirectToAction("Index", "Home");
        }

        var errorAlert = new Alert { Message = "Usuario o contraseña incorrectos", Type = "error" };
        TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(errorAlert);
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index), "Home");
    }
}