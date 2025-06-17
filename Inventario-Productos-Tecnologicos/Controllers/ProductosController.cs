using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Controllers
{
    public class ProductosController : Controller
    {
        private readonly TecnoCoreDbContext _context;

        public ProductosController(TecnoCoreDbContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Subcategoria)
                .Where(p => p.Activo == true)
                .ToListAsync();
            return View(productos);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Subcategoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["MarcaId"] = new SelectList(_context.Marcas.Where(m => m.Activo == true), "Id", "Nombre");
            ViewData["SubcategoriaId"] = new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio,Stock,Imagen,Novedad,MarcaId,SubcategoriaId")] Productos producto)
        {
            if (ModelState.IsValid)
            {
                producto.Activo = true;
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MarcaId"] = new SelectList(_context.Marcas.Where(m => m.Activo == true), "Id", "Nombre", producto.MarcaId);
            ViewData["SubcategoriaId"] = new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre", producto.SubcategoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            ViewData["MarcaId"] = new SelectList(_context.Marcas.Where(m => m.Activo == true), "Id", "Nombre", producto.MarcaId);
            ViewData["SubcategoriaId"] = new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre", producto.SubcategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,Stock,Imagen,Novedad,MarcaId,SubcategoriaId,Activo")] Productos producto)
        {
            if (id != producto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MarcaId"] = new SelectList(_context.Marcas.Where(m => m.Activo == true), "Id", "Nombre", producto.MarcaId);
            ViewData["SubcategoriaId"] = new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre", producto.SubcategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Subcategoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                // Soft delete (desactivar)
                producto.Activo = false;
                _context.Update(producto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(p => p.Id == id);
        }

        public IActionResult PorCategoria()
        {
            throw new NotImplementedException();
        }

        public IActionResult PorSubcategoria()
        {
            throw new NotImplementedException();
        }
    }
}
