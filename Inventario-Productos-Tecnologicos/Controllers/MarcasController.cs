using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;

namespace Inventario_Productos_Tecnologicos.Controllers
{
    public class MarcasController : Controller
    {
        private readonly TecnoCoreDbContext _context;

        public MarcasController(TecnoCoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var marcas = await _context.Marcas.ToListAsync();
            return View(marcas); // Esto llena el Model con una lista v�lida
        }

        public ViewResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Nombre", "Activo")] Marcas marca)
        {
            if (!ModelState.IsValid) return View(marca);
            try
            {
                _context.Marcas.Add(marca);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("", "Error al crear la marca. Intente nuevamente.");
                return View(marca);
            }
        }

        public async Task<IActionResult> Search(string searchElement, string activeFilter)
        {
            ViewBag.SearchString = searchElement;
            ViewBag.ActiveFilter = activeFilter;

            var query = _context.Marcas.AsQueryable();

            // Aplicar filtro de búsqueda si existe
            if (!string.IsNullOrEmpty(searchElement))
                query = query.Where(m => m.Nombre.Contains(searchElement)
                                         || m.Id.ToString().Contains(searchElement));

            // Aplicar filtro de estado si no es "all"
            if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
            {
                var isActive = activeFilter == "true";
                query = query.Where(m => m.Activo == isActive);
            }

            var marcas = await query.ToListAsync();
            return View("Index", marcas);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null) return NotFound();
            return View(marca);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] Marcas marca)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Datos inválidos enviados al servidor." });

                var marcaExistente = await _context.Marcas.FindAsync(marca.Id);
                if (marcaExistente == null)
                    return NotFound(new { message = "Marca no encontrada" });

                marcaExistente.Nombre = marca.Nombre;
                marcaExistente.Activo = marca.Activo;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la marca: " + ex.Message });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null) return NotFound();
            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
