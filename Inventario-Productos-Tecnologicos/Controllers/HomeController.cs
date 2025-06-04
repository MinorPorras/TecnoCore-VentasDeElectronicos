using System.Diagnostics;
    using Inventario_Productos_Tecnologicos.Data;
    using Microsoft.AspNetCore.Mvc;
    using Inventario_Productos_Tecnologicos.Models;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Controllers;
    
    /// <summary>
    /// Controlador que maneja la lógica principal de la aplicación, incluyendo el inicio de sesión y la navegación.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TecnoCoreDbContext _dbContext = new TecnoCoreDbContext();
    
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="HomeController"/>.
        /// </summary>
        /// <param name="logger">La instancia del logger para registrar eventos de la aplicación.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
    
        /// <summary>
        /// Muestra la vista principal del índice.
        /// </summary>
        /// <returns>La vista Index.</returns>
        public IActionResult Index()
        {
            return View();
        }
    
        /// <summary>
        /// Maneja el inicio de sesión del usuario y redirecciona según el rol.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="contrasena">La contraseña del usuario.</param>
        /// <returns>Una redirección a la acción correspondiente según el resultado del inicio de sesión.</returns>
        public RedirectToActionResult Login(string email, string contrasena)
        {
            // Descomentar y modificar el siguiente código para implementar la lógica de inicio de sesión:
            // var usuario = _dbContext.Usuarios.Include(usuario => usuario.RolNavigation).FirstOrDefault(u =>
            //     u.Email == email && u.Contrasena == contrasena && u.RolNavigation != null &&
            //     u.RolNavigation.Name == "Admin");
            // if (usuario == null)
            // {
            //     ViewBag.Error = "Usuario o contraseña incorrectos";
            // }
            // if (usuario?.RolNavigation?.Name == "Admin")
            // {
            //     return RedirectToAction("Index", "Mantenimiento");
            // }
            // return RedirectToAction("Index");
    
            return RedirectToAction("Mantenimiento");
        }
    
        /// <summary>
        /// Muestra la vista de mantenimiento.
        /// </summary>
        /// <returns>La vista Mantenimiento.</returns>
        public ViewResult Mantenimiento()
        {
            return View();
        }
    
        /// <summary>
        /// Muestra la vista de política de privacidad.
        /// </summary>
        /// <returns>La vista Privacy.</returns>
        public IActionResult Privacy()
        {
            return View();
        }
    
        /// <summary>
        /// Maneja los errores y muestra la vista de error.
        /// </summary>
        /// <returns>La vista Error con los detalles del error.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }