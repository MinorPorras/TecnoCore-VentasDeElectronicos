using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Inventario_Productos_Tecnologicos.Controllers
{
    public class KardexController : Controller
    {
        private readonly TecnoCoreDbContext _context;

        public KardexController(TecnoCoreDbContext context)
        {
            _context = context;
        }

        // GET: Kardex
        public async Task<IActionResult> Index()
        {
            var kardexes = await _context.Kardex
                .Include(k => k.Producto)
                .Include(k => k.TipoMovimientoKardex)
                .ToListAsync();
            return View(kardexes);
        }

        // GET: Kardex/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var kardex = await _context.Kardex
                .Include(k => k.Producto)
                .Include(k => k.TipoMovimientoKardex)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (kardex == null) return NotFound();

            return View(kardex);
        }

        // GET: Kardex/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Productos.Where(p => p.Activo == true), "Id", "Nombre");
            ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TipoMovimientoKardex.Where(t => t.Activo == true), "Id", "Tipo");
            return View();
        }

        // POST: Kardex/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductoId,Cantidad,Descripcion,Fecha,StockAnterior,StockActual,TipoMovimientoId,Activo")] Kardex kardex)
        {
            if (ModelState.IsValid)
            {
                // Se puede agregar lógica para calcular stock anterior y actual, si aplica

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Productos.Where(p => p.Activo == true), "Id", "Nombre", kardex.ProductoId);
            ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TipoMovimientoKardex.Where(t => t.Activo == true), "Id", "Tipo", kardex.TipoMovimientoId);
            return View(kardex);
        }

        // GET: Kardex/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var kardex = await _context.Kardex.FindAsync(id);
            if (kardex == null) return NotFound();

            ViewData["ProductoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Productos.Where(p => p.Activo == true), "Id", "Nombre", kardex.ProductoId);
            ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TipoMovimientoKardex.Where(t => t.Activo == true), "Id", "Tipo", kardex.TipoMovimientoId);
            return View(kardex);
        }

        // POST: Kardex/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductoId,Cantidad,Descripcion,Fecha,StockAnterior,StockActual,TipoMovimientoId,Activo")] Kardex kardex)
        {
            if (id != kardex.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kardex);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KardexExists(kardex.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Productos.Where(p => p.Activo == true), "Id", "Nombre", kardex.ProductoId);
            ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TipoMovimientoKardex.Where(t => t.Activo == true), "Id", "Tipo", kardex.TipoMovimientoId);
            return View(kardex);
        }

        // GET: Kardex/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var kardex = await _context.Kardex
                .Include(k => k.Producto)
                .Include(k => k.TipoMovimientoKardex)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (kardex == null) return NotFound();

            return View(kardex);
        }

        // POST: Kardex/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kardex = await _context.Kardex.FindAsync(id);
            if (kardex != null)
            {
                _context.Kardex.Remove(kardex);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KardexExists(int id)
        {
            return _context.Kardex.Any(e => e.Id == id);
        }
    }
}