using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Inventario_Productos_Tecnologicos.Controllers;

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
            .ToListAsync();
        ViewBag.Marcas = new SelectList(_context.Marcas, "Id", "Nombre");
        ViewBag.Subcategorias = new SelectList(_context.Subcategorias, "Id", "Nombre");
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
        ViewData["SubcategoriaId"] =
            new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre");
        return View();
    }

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre,Descripcion,Precio,Stock,Novedad,MarcaId,SubcategoriaId")]
        Productos producto, IFormFile imagen)
    {
        if (imagen.Length > 0)
        {
            // Genera un nombre único para el archivo
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);

            // Define la ruta donde se guardará
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/productos");
            var rutaGuardado = Path.Combine(uploadPath, fileName);

            // Crear el directorio si no existe
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            // Guarda el archivo físicamente
            await using (var stream = new FileStream(rutaGuardado, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            // Guarda la ruta relativa en el modelo
            producto.Imagen = "/img/productos/" + fileName;
        }

        if (ModelState.IsValid)
        {
            producto.Activo = true;
            _context.Add(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["MarcaId"] =
            new SelectList(_context.Marcas.Where(m => m.Activo == true), "Id", "Nombre", producto.MarcaId);
        ViewData["SubcategoriaId"] = new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre",
            producto.SubcategoriaId);
        return View(producto);
    }

    // GET: Productos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();

        ViewData["MarcaId"] =
            new SelectList(_context.Marcas.Where(m => m.Activo == true), "Id", "Nombre", producto.MarcaId);
        ViewData["SubcategoriaId"] = new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre",
            producto.SubcategoriaId);
        return View(producto);
    }

    // POST: Productos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Nombre,Descripcion,Precio,Stock,Imagen,Novedad,MarcaId,SubcategoriaId,Activo")]
        Productos producto,
        IFormFile? imagen)
    {
        if (id != producto.Id) return NotFound();

        if (ModelState.IsValid)
            try
            {
                // Si se subió una nueva imagen
                if (imagen != null && imagen.Length > 0)
                {
                    // Genera un nombre único para el archivo
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);

                    // Define la ruta donde se guardará
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/productos");
                    var rutaGuardado = Path.Combine(uploadPath, fileName);

                    // Crear el directorio si no existe
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    // Guarda el archivo físicamente
                    await using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                    {
                        await imagen.CopyToAsync(stream);
                    }

                    // Guarda la ruta relativa en el modelo
                    producto.Imagen = "/img/productos/" + fileName;
                }
                else
                {
                    // Obtener el producto existente para mantener la imagen actual
                    var existingProduct = await _context.Productos.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct != null) producto.Imagen = existingProduct.Imagen;
                }

                _context.Update(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(producto.Id))
                    return NotFound();
                else
                    throw;
            }

        ViewData["MarcaId"] =
            new SelectList(_context.Marcas.Where(m => m.Activo == true), "Id", "Nombre", producto.MarcaId);
        ViewData["SubcategoriaId"] = new SelectList(_context.Subcategorias.Where(s => s.Activo == true), "Id", "Nombre",
            producto.SubcategoriaId);
        return View(producto);
    }

    // POST: Productos/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto != null)
        {
            // Soft delete (desactivar)
            producto.Activo = !producto.Activo;
            _context.Update(producto);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ProductoExists(int id)
    {
        return _context.Productos.Any(p => p.Id == id);
    }

    public async Task<IActionResult> Search(string searchElement, string marcas, string subcategorias,
        string activeFilter)
    {
        ViewBag.SearchString = searchElement;
        ViewBag.SelectedMarca = marcas ?? "all";
        ViewBag.SelectedSubcategoria = subcategorias ?? "all";
        ViewBag.ActiveFilter = activeFilter;

        // Obtener las listas para los dropdowns
        ViewBag.marcas =
            new SelectList(await _context.Marcas.Where(m => m.Activo == true).ToListAsync(), "Id", "Nombre");
        ViewBag.subcategorias = new SelectList(await _context.Subcategorias.Where(s => s.Activo == true).ToListAsync(),
            "Id", "Nombre");

        var query = _context.Productos
            .Include(p => p.Marca)
            .Include(p => p.Subcategoria)
            .Where(p => p.Activo == true); // Filtro inicial de productos activos

        // Aplicar filtro de búsqueda si existe
        if (!string.IsNullOrEmpty(searchElement))
            query = query.Where(p => p.Nombre.ToLower().Contains(searchElement.ToLower())
                                     || p.Id.ToString().Contains(searchElement));

        // Aplicar filtro de estado si no es "all"
        if (marcas != "all" && !string.IsNullOrEmpty(marcas)) query = query.Where(p => p.MarcaId == int.Parse(marcas));

        // Aplicar filtro de rol si no es "all"
        if (subcategorias != "all" && !string.IsNullOrEmpty(subcategorias))
            query = query.Where(p => p.SubcategoriaId == int.Parse(subcategorias));

        if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
        {
            var isActive = activeFilter == "true";
            query = query.Where(p => p.Activo == isActive);
        }

        var productos = await query.ToListAsync();
        return View("Index", productos);
    }

    public async Task<IActionResult> CreateKardexEntry(int id)
    {
        var viewModel = new Models.ViewModels.KardexViewModel
        {
            Fecha = DateTime.Now,
            ProductosDisponibles = await _context.Productos
                .Where(p => p.Activo == true)
                .ToListAsync()
        };

        ViewData["TipoMovimientoId"] =
            new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.TipoMovimientoKardex.Where(t => t.Activo == true && t.Entrada).ToListAsync(),
                "Id", "Tipo");

        ViewBag.Producto = await _context.Productos.FindAsync(id);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateKardexEntry(Models.ViewModels.KardexViewModel viewModel)
    {
        if (ModelState.IsValid)
            try
            {
                var kardex = new Kardex
                {
                    ProductoId = viewModel.ProductoId,
                    Fecha = viewModel.Fecha,
                    TipoMovimientoId = viewModel.TipoMovimientoId,
                    Cantidad = viewModel.Cantidad,
                    Descripcion = viewModel.Descripcion,
                    Activo = viewModel.Activo
                };

                if (kardex.ProductoId.HasValue)
                {
                    var producto = await _context.Productos.FindAsync(kardex.ProductoId);
                    if (producto != null)
                    {
                        kardex.StockAnterior = producto.Stock;
                        var tipoMovimiento = await _context.TipoMovimientoKardex.FindAsync(kardex.TipoMovimientoId);
                        if (tipoMovimiento != null && tipoMovimiento.Entrada)
                        {
                            producto.Stock += kardex.Cantidad ?? 0;
                            kardex.StockActual = producto.Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Kardex", new { id = kardex.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el movimiento de kardex: " + ex.Message);
            }

        viewModel.ProductosDisponibles = await _context.Productos
            .Where(p => p.Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new SelectList(
            await _context.TipoMovimientoKardex.Where(t => t.Activo == true && t.Entrada).ToListAsync(),
            "Id", "Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    public async Task<IActionResult> CreateKardexExit(int id)
    {
        var viewModel = new Models.ViewModels.KardexViewModel
        {
            Fecha = DateTime.Now,
            ProductosDisponibles = await _context.Productos
                .Where(p => p.Activo == true)
                .ToListAsync()
        };

        ViewData["TipoMovimientoId"] =
            new SelectList(
                await _context.TipoMovimientoKardex.Where(t => t.Activo == true && !t.Entrada).ToListAsync(),
                "Id", "Tipo");
        
        ViewBag.Producto = await _context.Productos.FindAsync(id);
        
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateKardexExit(Models.ViewModels.KardexViewModel viewModel)
    {
        if (ModelState.IsValid)
            try
            {
                var kardex = new Kardex
                {
                    ProductoId = viewModel.ProductoId,
                    Fecha = viewModel.Fecha,
                    TipoMovimientoId = viewModel.TipoMovimientoId,
                    Cantidad = viewModel.Cantidad,
                    Descripcion = viewModel.Descripcion,
                    Activo = viewModel.Activo
                };

                if (kardex.ProductoId.HasValue)
                {
                    var producto = await _context.Productos.FindAsync(kardex.ProductoId);
                    if (producto != null)
                    {
                        kardex.StockAnterior = producto.Stock;
                        var tipoMovimiento = await _context.TipoMovimientoKardex.FindAsync(kardex.TipoMovimientoId);
                        if (tipoMovimiento != null && !tipoMovimiento.Entrada)
                        {
                            producto.Stock += kardex.Cantidad ?? 0;
                            kardex.StockActual = producto.Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Kardex", new { id = kardex.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el movimiento de kardex: " + ex.Message);
            }

        viewModel.ProductosDisponibles = await _context.Productos
            .Where(p => p.Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.TipoMovimientoKardex.Where(t => t.Activo == true && !t.Entrada).ToListAsync(),
            "Id", "Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductoStock(int id)
    {
        var stock = await _context.Productos
            .Where(p => p.Id == id)
            .Select(p => new { stock = p.Stock })
            .FirstOrDefaultAsync();

        if (stock == null) return NotFound();
        return Json(stock);
    }

    [HttpGet]
    public async Task<IActionResult> GetTipoMovimiento(int id)
    {
        var tipoMovimiento = await _context.TipoMovimientoKardex
            .Where(t => t.Id == id)
            .Select(t => new { esEntrada = t.Entrada })
            .FirstOrDefaultAsync();

        if (tipoMovimiento == null) return NotFound();
        return Json(tipoMovimiento);
    }
}