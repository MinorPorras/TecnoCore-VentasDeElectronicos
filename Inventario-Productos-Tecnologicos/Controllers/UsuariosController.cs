using Microsoft.AspNetCore.Mvc;
    
    namespace Inventario_Productos_Tecnologicos.Controllers;
    
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los usuarios del sistema.
    /// </summary>
    public class UsuariosController : Controller
    {
        /// <summary>
        /// Muestra la vista principal de usuarios.
        /// </summary>
        /// <returns>La vista Index de usuarios.</returns>
        public IActionResult Index()
        {
            return View();
        }
    
        /// <summary>
        /// Muestra la información personal de un usuario específico.
        /// </summary>
        /// <param name="id">El identificador único del usuario.</param>
        /// <returns>La vista con la información personal del usuario.</returns>
        public IActionResult Informacion_Personal(int id)
        {
            return View();
        }
        ///<summary>
        /// Muestra la lista de deseos de un usuario específico.
        /// </summary>
        /// <param name= "id"/>El identificador único del usuario.
        /// <returns>La vista con la lista de deseos del usuario.</returns>
        public IActionResult Lista_Deseos(int id)
        {
            return View();
        }
    }