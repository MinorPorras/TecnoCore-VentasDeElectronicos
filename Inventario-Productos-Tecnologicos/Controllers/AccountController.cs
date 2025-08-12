using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Identity; // Necesario para UserManager y SignInManager
using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models; // Tu modelo Usuarios, Provincia, Canton, Direccion
using Inventario_Productos_Tecnologicos.Models.ViewModels; // Tu RegisterViewModel
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Authorization; // Tu DbContext
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
    private readonly RoleManager<TECO_A_Roles> _roleManager;

    public AccountController(TecnoCoreDbContext context,
        UserManager<TECO_A_Usuario> userManager, SignInManager<TECO_A_Usuario> signInManager,
        ILogger<AccountController> logger, RoleManager<TECO_A_Roles> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _roleManager = roleManager;
    }

    [Authorize(Roles = "Administrador")]
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
        // Si el modelo no es válido, volvemos a mostrar el formulario con los errores.
        if (!ModelState.IsValid)
        {
            ViewBag.Alert = Alert.ErrorAlert("Por favor, corrija los errores en el formulario.");
            _logger.LogCritical("Modelo: " + JsonSerializer.Serialize(model));
            await RellenarProvinciasCantones(model);
            return View(model);
        }
        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Verificación de nombre de usuario existente
                var userNameExists = await _userManager.FindByNameAsync(model.UserName);
                if (userNameExists != null)
                {
                    _logger.LogCritical("El nombre de usuario ya existe");
                    ViewBag.Alert = Alert.ErrorAlert($"El usuario ya existe, escoja otro");
                    await RellenarProvinciasCantones(model);
                    return View(model);
                }
                // Verificación de correo electrónico existente
                var emailExists = await _userManager.FindByEmailAsync(model.Email);
                if (emailExists != null)
                {
                    _logger.LogCritical("El correo ya está registrado");
                    ViewBag.Alert = Alert.ErrorAlert($"El correo ya está registrado, escoja otro");
                    await RellenarProvinciasCantones(model);
                    return View(model);
                }
                var user = new TECO_A_Usuario
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    TC_Nombre = model.Nombre,
                    TC_Apellidos = model.Apellidos, // El campo único para apellidos
                    PhoneNumber = model.PhoneNumber.Replace("-", ""),
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
                        ViewBag.Alert = Alert.ErrorAlert("El cantón seleccionado no es válido. La dirección no pudo ser guardada.");
                        await transaction.RollbackAsync(); // Revertimos la creación del usuario.
                        await RellenarProvinciasCantones(model);
                        return View(model);
                    }
                    else
                    {
                        TempData["Error"] = JsonSerializer.Serialize(
                            Alert.ErrorAlert($"No se encotró la provincia o el cantón"));
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

                    TempData["success"] = JsonSerializer.Serialize(Alert.InfoAlert($"Registro exitoso"));

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

                TempData["Alert"] = JsonSerializer.Serialize(Alert.ErrorAlert($"Error al registrar el usuario"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error durante el registro de usuario");
                var errorAlert = new Alert { Message = "Error interno del servidor", Type = "error" };
                TempData["Alert"] = JsonSerializer.Serialize(Alert.ErrorAlert($"Error interno del servidor: {e.Message}"));
                await transaction.RollbackAsync();
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
            var alert = new Alert { Message = "Credenciales inválidas", Type = "error" };
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(alert);
            return View(model);
        }

        // Primero verificamos si el usuario existe y está activo
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user != null && !user.TB_Activo)
        {
            _logger.LogWarning("Intento de inicio de sesión de usuario inactivo: {Username}", model.UserName);
            var inactiveAlert = new Alert
                { Message = "Su cuenta se encuentra inactiva. Por favor contacte al administrador.", Type = "error" };
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(inactiveAlert);
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

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index), "Home");
    }

    public async Task<IActionResult> EditAccountInfo()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usuario = await _userManager.Users
            .Include(u => u.Direccion)
            .ThenInclude(d => d.Canton)
            .FirstOrDefaultAsync(u => u.Id == usuarioId);        
        if (usuario == null)
        {
            TempData["Error"] = JsonSerializer.Serialize(Alert.InfoAlert("Usuario no encontrado"));
            return RedirectToAction(nameof(Index));
        }
        var model = new EditUserViewModel()
        {
            Id = usuarioId,
            //Datos de la cuenta
            UserName = usuario.UserName ?? "No econtrado",
            Email = usuario.Email ?? "No econtrado",
            
            //Datos personales
            Nombre = usuario.TC_Nombre,
            Apellidos = usuario.TC_Apellidos,
            PhoneNumber = usuario.PhoneNumber ?? "",
            
            //Dirección
            SelectedProvinciaId = usuario.Direccion?.Canton?.TN_ProvinciaId ?? 0,
            SelectedCantonId = usuario.Direccion?.TN_CantonId ?? 0,               
            DireccionExacta = usuario.Direccion?.TC_Direccion ?? "",              
            CodigoPostal = usuario.Direccion?.TC_CodigoPostal ?? ""  
        };
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
        _logger.LogCritical("Usuario: " + JsonSerializer.Serialize(model));
        return View(model);
    }

   [HttpPut]
   [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAccountInfo([FromBody] EditUserViewModel model)
    {
        try
        {
            _logger.LogCritical("Datos nuevos: " + JsonSerializer.Serialize(model));
            _logger.LogInformation("Editing usuario");
            _logger.LogCritical("{ModelStateIsValid}", ModelState.IsValid);
            
            _logger.LogCritical("UserName encontrado: {UserName}", model.UserName);
            _logger.LogCritical("Email encontrado: {Email}", model.Email);
            _logger.LogCritical("Nombre encontrado: {Nombre}", model.Nombre);
            _logger.LogCritical("Apellidos encontrado: {Apellidos}", model.Apellidos);
            _logger.LogCritical("PhoneNumber encontrado: {PhoneNumber}", model.PhoneNumber);

            if (!ModelState.IsValid)
            {
                // Recolectar todos los mensajes de error
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Crear un mensaje combinado con todos los errores
                var errorMessage = string.Join("\n• ", errorMessages);
                errorMessage = "Se encontraron los siguientes errores:\n• " + errorMessage;

                // Recargar los datos necesarios para el formulario
                reloadFormElements(model);
                return BadRequest(new { success = false, message = errorMessage});
            }

            //Busca el usuario y si no lo encuentra devuelve un mensaje de error
            _logger.LogCritical("Usuario encontrado: {Id}", User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (usuario == null)
            {
                return NotFound(new { success = false, message = "No se encontró el usuario a modificar"});
            }


            usuario.UserName = model.UserName;
            usuario.Email = model.Email;
            usuario.TC_Nombre = model.Nombre;
            usuario.TC_Apellidos = model.Apellidos;
            usuario.PhoneNumber = model.PhoneNumber;
            
            _logger.LogCritical("UserName encontrado: {UserName}", model.UserName);
            _logger.LogCritical("Email encontrado: {Email}", model.Email);
            _logger.LogCritical("Nombre encontrado: {Nombre}", model.Nombre);
            _logger.LogCritical("Apellidos encontrado: {Apellidos}", model.Apellidos);
            _logger.LogCritical("PhoneNumber encontrado: {PhoneNumber}", model.PhoneNumber);



            //Actualizar el usuario
            var result = await _userManager.UpdateAsync(usuario);

            //Si el usuario se actualiza correctamente actualiza la demás información sino devuelve la vista de edición
            if (result.Succeeded)
            {
                _logger.LogCritical("Se guardó la información del cliente");
                // Actualizar la dirección del usuario
                var direccion = await _context.TECO_A_Direccion
                    .FirstOrDefaultAsync(d => d.TN_UsuarioId == usuario.Id);
                _logger.LogCritical("Dirección encontrada: {DireccionId}", direccion?.TN_Id);
                _logger.LogCritical("Cantón seleccionado: {SelectedCantonId}", model.SelectedCantonId);
                if (direccion == null)
                {
                    // Si la dirección NO existe, la CREAMOS y la agregamos al contexto
                    direccion = new TECO_A_Direccion
                    {
                        TN_UsuarioId = usuario.Id,
                        TB_Activo = true,
                        TC_Direccion = model.DireccionExacta,
                        TC_CodigoPostal = model.CodigoPostal,
                        TN_CantonId = model.SelectedCantonId ?? 0
                    };
                    _context.TECO_A_Direccion.Add(direccion);
                }
                else
                {
                    direccion.TC_Direccion = model.DireccionExacta;
                    direccion.TC_CodigoPostal = model.CodigoPostal;
                    direccion.TN_CantonId = model.SelectedCantonId ?? 0;
                }

                await _context.SaveChangesAsync();
                TempData["success"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.SuccessAlert());
                 return Ok(new { success = true, redirectUrl = Url.Action("Informacion_personal", "Usuario") });            }
            else
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Error al actualizar el usuario: " +
                                     string.Join(", ", result.Errors.Select(e => e.Description))));

                // Recargar los datos necesarios para el formulario
                reloadFormElements(model);
                return View(model);
            }
        }
        catch (Exception ex)
        {
            // En caso de excepción, intenta rellenar para que la vista pueda renderizarse
            // Esto podría fallar si 'model' es nulo, así que añade una comprobación
            if (model != null)
            {
                reloadFormElements(model);
            }
            _logger.LogError(ex, "Error al editar usuario");
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert($"Error al editar el usuario: {ex.Message}"));
            return View(model ?? new EditUserViewModel()); // Devuelve un nuevo modelo vacío si 'model' es nulo
        }
    }
    
    private void reloadFormElements(EditUserViewModel model)
    {
        try
        {
            // Recargar los datos necesarios para el formulario
            var roles = _roleManager.Roles.Where(r => r.TB_Activo).ToList();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            model.Provincias =
                new List<SelectListItem>(new SelectList(_context.TECO_M_Provincia.OrderBy(p => p.TC_Nombre),
                    "TN_Id",
                    "TC_Nombre"));
            if (model.SelectedProvinciaId > 0)
                model.Cantones =
                [
                    ..new SelectList(
                        _context.TECO_M_Canton.Where(c => c.TN_ProvinciaId == model.SelectedProvinciaId),
                        "TN_Id", "TC_Nombre")
                ];
            else
                model.Cantones =
                [
                    ..new SelectList(Enumerable.Empty<TECO_M_Canton>(), "TN_Id", "TC_Nombre")
                ];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

}