using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Models.ViewModels;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class ProductosController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductosController(TecnoCoreDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
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
        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
        producto.Imagen = GetImagePath(producto.Imagen);

        return View(producto);
    }

    private string GetImagePath(string? imagePath)
    {
        const string defaultImageUrl = "/img/productos/default-image.jpg";
        if (string.IsNullOrEmpty(imagePath)) return defaultImageUrl;
        // Convertir la ruta relativa de la BD a ruta física
        var rutaFisica = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
        // Verificar si el archivo existe y devolver la ruta URL relativa original
        if (System.IO.File.Exists(rutaFisica)) return imagePath;
        return defaultImageUrl;
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
        else
        {
            producto.Imagen = "/img/productos/default-image.jpg";
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
        var viewModel = new KardexViewModel
        {
            Fecha = DateTime.Now,
            ProductosDisponibles = await _context.Productos
                .Where(p => p.Activo == true)
                .ToListAsync()
        };

        ViewData["TipoMovimientoId"] =
            new SelectList(
                await _context.TipoMovimientoKardex.Where(t => t.Activo == true && t.Entrada).ToListAsync(),
                "Id", "Tipo");

        ViewBag.Producto = await _context.Productos.FindAsync(id);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateKardexEntry(KardexViewModel viewModel)
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
        var viewModel = new KardexViewModel
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
    public async Task<IActionResult> CreateKardexExit(KardexViewModel viewModel)
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

        ViewData["TipoMovimientoId"] = new SelectList(
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

    public async Task<IActionResult> ListaProductos(ProductListViewModel filterModel, int categoriaId = 0,
        int subcategoriaId = 0)
    {
        //Persistencia en para los ID de navegación de categoría y subcategoría
        Console.WriteLine("Categoría: " + categoriaId);
        Console.WriteLine("Subcategoría: " + subcategoriaId);
        filterModel.CurrentCategoriaId = categoriaId;
        filterModel.CurrentSubcategoriaId = subcategoriaId;

        //Inicializar la consulta con la DB mostrando solo productos activos
        var query = _context.Productos
            .Include(p => p.Marca)
            .Include(p => p.Subcategoria)
            .ThenInclude(s => s.Categoria)
            .Where(p => p.Activo);

        // Aplicar filtros básicos de búsqueda
        if (categoriaId != 0)
        {
            query = query.Where(p => p.Subcategoria.CategoriaId == categoriaId);
            ViewBag.Title = await _context.Categorias
                .Where(c => c.Id == categoriaId)
                .Select(c => c.Nombre)
                .FirstOrDefaultAsync() ?? "Productos";
        }

        if (subcategoriaId != 0)
        {
            query = query.Where(p => p.SubcategoriaId == subcategoriaId);
            ViewBag.Title = await _context.Subcategorias
                .Where(s => s.Id == subcategoriaId)
                .Select(s => s.Nombre)
                .FirstOrDefaultAsync() ?? "Productos";
        }

        // Aplicar filtro de búsqueda global por termino (Desde el layout)
        if (!string.IsNullOrEmpty(filterModel.SearchTerm))
        {
            query = query.Where(p => p.Nombre.ToLower().Contains(filterModel.SearchTerm.ToLower())
                                     || p.Descripcion.ToLower().Contains(filterModel.SearchTerm.ToLower()));
            ViewBag.CurrentSearch = filterModel.SearchTerm;
        }
        else
        {
            ViewBag.CurrentSearch = string.Empty;
        }

        // Filtrar por categorías y subcategorías seleccionadas
        // Filtrar por Marcas seleccionadas
        if (filterModel.SelectedMarcaIds.Any())
            query = query.Where(p => filterModel.SelectedMarcaIds.Contains(p.Marca.Id));
        

        // Filtrar por Categorías seleccionadas (desde el sidebar)
        if (filterModel.SelectedCategoriaIds.Any())
            query = query.Where(p => p.Subcategoria != null && filterModel.SelectedCategoriaIds.Contains(p.Subcategoria.Categoria.Id));
        

        // Filtrar por Subcategorías seleccionadas (desde el sidebar)
        if (filterModel.SelectedSubcategoryIds.Any())
            query = query.Where(p => p.Subcategoria != null && filterModel.SelectedSubcategoryIds.Contains(p.Subcategoria.Id));

        // Aplicar ordenamiento
        if (string.IsNullOrEmpty(filterModel.OrderBy)) filterModel.OrderBy = "name";
        if (string.IsNullOrEmpty(filterModel.OrderForm)) filterModel.OrderForm = "asc";

        switch (filterModel.OrderBy)
        {
            case "name":
                query = filterModel.OrderForm == "asc"
                    ? query.OrderBy(p => p.Nombre)
                    : query.OrderByDescending(p => p.Nombre);
                break;
            case "price":
                query = filterModel.OrderForm == "asc"
                    ? query.OrderBy(p => p.Precio)
                    : query.OrderByDescending(p => p.Precio);
                break;
            default:
                query = query.OrderBy(p => p.Nombre);
                break;
        }

        // Obtener la lista de productos filtrados
        filterModel.ProductosList = await query.ToListAsync();
        var marcasIds = filterModel.ProductosList.Select(p => p.MarcaId).Distinct().ToList();
        filterModel.ProductosMarcas = await _context.Marcas
            .Where(m => marcasIds.Contains(m.Id) && m.Activo)
            .ToListAsync(); 
        var categoriasIds = filterModel.ProductosList
            .Select(p => p.Subcategoria.CategoriaId)
            .Distinct()
            .ToList();
        filterModel.ProductosCategorias = await _context.Categorias
            .Where(c => categoriasIds.Contains(c.Id) && c.Activo)
            .ToListAsync();

        // Las subcategorías deben mostrarse según la categoría seleccionada si hay una categoríaId inicial
        // O si se ha seleccionado alguna categoría en los filtros del sidebar.
        if (categoriaId > 0)
        {
            filterModel.ProductosSubcategorias = await _context.Subcategorias
                .Where(s => s.CategoriaId == categoriaId && s.Activo)
                .ToListAsync();
        }
        else if (filterModel.SelectedCategoriaIds.Any()) // Si se filtró por categorías en el sidebar
        {
            filterModel.ProductosSubcategorias = await _context.Subcategorias
                .Where(s => filterModel.SelectedCategoriaIds.Contains(s.Categoria.Id) && s.Activo)
                .ToListAsync();
        }
        else // Si no hay filtro de categoría, mostrar todas las subcategorías activas
        {
            filterModel.ProductosSubcategorias = await _context.Subcategorias.Where(s => s.Activo).ToListAsync();
        }

        //Manejar mensajes si no hay productos
        if (!filterModel.ProductosList.Any())
            ViewBag.Message = "No se encontraron productos con los filtros aplicados.";
        else
            ViewBag.Message = null; // Limpiar el mensaje si hay productos

        return View(filterModel);
    }
}