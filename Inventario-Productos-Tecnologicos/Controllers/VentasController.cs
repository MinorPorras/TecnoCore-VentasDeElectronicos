using Microsoft.AspNetCore.Mvc;
using Inventario_Productos_Tecnologicos.Models;
        
        namespace Inventario_Productos_Tecnologicos.Controllers;
        
        /// <summary>
        /// Controlador que maneja las operaciones relacionadas con las ventas del sistema.
        /// </summary>
        public class VentasController : Controller
        {
            /// <summary>
            /// Muestra la vista principal de ventas.
            /// </summary>
            /// <returns>La vista Index de ventas.</returns>
            public IActionResult Index()
            {
                return View();
            }
        
            /// <summary>
            /// Muestra la vista de gestión de cupones de descuento.
            /// </summary>
            /// <returns>La vista Cupones para administrar cupones de descuento.</returns>
            public IActionResult Cupones()
            {
                return View();
            }
        
            /// <summary>
            /// Muestra la vista de gestión de métodos de pago disponibles.
            /// </summary>
            /// <returns>La vista de métodos de pago para su administración.</returns>
            public IActionResult Metodos_de_Pago()
            {
                return View();
            }

            public IActionResult Carro_Compras(List<Producto> listaCompras)
            {
                return View();
            }
        }