using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Data;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inventario_Productos_Tecnologicos.Models.ViewModels;

namespace Inventario_Productos_Tecnologicos.Controllers;

public class ProductosController : Controller
{
    private readonly TecnoCoreDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(TecnoCoreDbContext context,
        IWebHostEnvironment webHostEnvironment,
        ILogger<ProductosController> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    // GET: Productos
    public async Task<IActionResult> Index()
    {
        var productos = await _context.TECO_A_Producto
            .Include(p => p.Marca)
            .Include(p => p.Subcategoria)
            .ToListAsync();
        ViewBag.Marcas = new SelectList(_context.TECO_M_Marca, "TN_Id", "TC_Nombre");
        ViewBag.Subcategorias = new SelectList(_context.TECO_M_Subcategoria, "TN_Id", "TC_Nombre");
        return View(productos);
    }

    // GET: Productos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("ID de producto no válido"));
            return NotFound();
        }

        var producto = await _context.TECO_A_Producto
            .Include(p => p.Marca)
            .Include(p => p.Subcategoria)
            .FirstOrDefaultAsync(p => p.TN_Id == id);

        if (producto == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.NotFoundAlert("el producto"));
            return NotFound();
        }

        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
        producto.TC_Imagen = GetImagePath(producto.TC_Imagen);

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
        ViewData["MarcaId"] = new SelectList(_context.TECO_M_Marca.Where(m => m.TB_Activo), "TN_Id", "TC_Nombre");
        ViewData["SubcategoriaId"] =
            new SelectList(_context.TECO_M_Subcategoria.Where(s => s.TB_Activo), "TN_Id", "TC_Nombre");
        return View();
    }

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("TC_Nombre,TC_Descripcion,TN_Precio,TN_Stock,TB_Novedad,TN_MarcaId,TN_SubcategoriaId")]
        TECO_A_Producto producto, IFormFile TC_Imagen)
    {
        if (TC_Imagen.Length > 0)
        {
            // Genera un nombre único para el archivo
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(TC_Imagen.FileName);

            // Define la ruta donde se guardará
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/productos");
            var rutaGuardado = Path.Combine(uploadPath, fileName);

            // Crear el directorio si no existe
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            // Guarda el archivo físicamente
            await using (var stream = new FileStream(rutaGuardado, FileMode.Create))
            {
                await TC_Imagen.CopyToAsync(stream);
            }

            // Guarda la ruta relativa en el modelo
            producto.TC_Imagen = "/img/productos/" + fileName;
        }
        else
        {
            producto.TC_Imagen = "/img/productos/default-image.jpg";
        }

        if (ModelState.IsValid)
        {
            producto.TB_Activo = true;
            _context.Add(producto);
            await _context.SaveChangesAsync();
            TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
            return RedirectToAction(nameof(Index));
        }

        ViewData["MarcaId"] = new SelectList(_context.TECO_M_Marca.Where(m => m.TB_Activo), "TN_Id", "TC_Nombre",
            producto.TN_MarcaId);
        ViewData["SubcategoriaId"] = new SelectList(_context.TECO_M_Subcategoria.Where(s => s.TB_Activo), "TN_Id",
            "TC_Nombre", producto.TN_SubcategoriaId);
        TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(Alert.ErrorAlert("Error al crear el producto"));
        return View(producto);
    }

    // GET: Productos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(Alert.ErrorAlert("ID de producto no válido"));
            return NotFound();
        }

        var producto = await _context.TECO_A_Producto.FindAsync(id);
        if (producto == null)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(Alert.NotFoundAlert("el producto"));
            return NotFound();
        }

        ViewData["MarcaId"] = new SelectList(_context.TECO_M_Marca.Where(m => m.TB_Activo), "TN_Id", "TC_Nombre",
            producto.TN_MarcaId);
        ViewData["SubcategoriaId"] = new SelectList(_context.TECO_M_Subcategoria.Where(s => s.TB_Activo), "TN_Id",
            "TC_Nombre", producto.TN_SubcategoriaId);
        return View(producto);
    }

    // POST: Productos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind(
            "TN_Id,TC_Nombre,TC_Descripcion,TN_Precio,TN_Stock,TC_Imagen,TB_Novedad,TN_MarcaId,TN_SubcategoriaId,TB_Activo")]
        TECO_A_Producto producto,
        IFormFile? imagen)
    {
        if (id != producto.TN_Id)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("ID de producto no válido"));
            return NotFound();
        }

        if (ModelState.IsValid)
            try
            {
                if (imagen != null && imagen.Length > 0)
                {
                    // Genera un nombre único para el archivo
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/productos");
                    var rutaGuardado = Path.Combine(uploadPath, fileName);

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    await using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                    {
                        await imagen.CopyToAsync(stream);
                    }

                    producto.TC_Imagen = "/img/productos/" + fileName;
                }
                else
                {
                    var existingProduct = await _context.TECO_A_Producto.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.TN_Id == id);
                    if (existingProduct != null)
                        producto.TC_Imagen = existingProduct.TC_Imagen;
                }

                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["success"] = System.Text.Json.JsonSerializer.Serialize(Alert.SuccessAlert());
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(producto.TN_Id))
                {
                    TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                        Alert.NotFoundAlert("el producto"));
                    return NotFound();
                }

                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Error de concurrencia al actualizar el producto"));
                throw;
            }
            catch (Exception ex)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.ErrorAlert("Error al actualizar el producto: " + ex.Message));
                _logger.LogError(ex, "Error al actualizar el producto {ProductoId}", id);
            }

        ViewData["MarcaId"] = new SelectList(_context.TECO_M_Marca.Where(m => m.TB_Activo), "TN_Id", "TC_Nombre",
            producto.TN_MarcaId);
        ViewData["SubcategoriaId"] = new SelectList(_context.TECO_M_Subcategoria.Where(s => s.TB_Activo), "TN_Id",
            "TC_Nombre", producto.TN_SubcategoriaId);
        TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
            Alert.ErrorAlert("Por favor, revise los datos ingresados"));
        return View(producto);
    }

    // POST: Productos/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchActive(int id)
    {
        try
        {
            var producto = await _context.TECO_A_Producto.FindAsync(id);
            if (producto == null)
            {
                TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.NotFoundAlert("el producto"));
                return RedirectToAction(nameof(Index));
            }

            producto.TB_Activo = !producto.TB_Activo;
            _context.Update(producto);
            await _context.SaveChangesAsync();

            TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.InfoAlert("Estado del producto cambiado correctamente"));
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cambiar el estado del producto"));
            _logger.LogError(ex, "Error al cambiar el estado del producto {ProductoId}", id);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ProductoExists(int id)
    {
        return _context.TECO_A_Producto.Any(p => p.TN_Id == id);
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
            new SelectList(await _context.TECO_M_Marca.Where(m => m.TB_Activo == true).ToListAsync(), "Id", "Nombre");
        ViewBag.subcategorias = new SelectList(
            await _context.TECO_M_Subcategoria.Where(s => s.TB_Activo == true).ToListAsync(),
            "Id", "Nombre");

        var query = _context.TECO_A_Producto
            .Include(p => p.Marca)
            .Include(p => p.Subcategoria)
            .Where(p => p.TB_Activo == true); // Filtro inicial de productos activos

        // Aplicar filtro de búsqueda si existe
        if (!string.IsNullOrEmpty(searchElement))
            query = query.Where(p => p.TC_Nombre.ToLower().Contains(searchElement.ToLower())
                                     || p.TN_Id.ToString().Contains(searchElement));

        // Aplicar filtro de estado si no es "all"
        if (marcas != "all" && !string.IsNullOrEmpty(marcas))
            query = query.Where(p => p.TN_MarcaId == int.Parse(marcas));

        // Aplicar filtro de rol si no es "all"
        if (subcategorias != "all" && !string.IsNullOrEmpty(subcategorias))
            query = query.Where(p => p.TN_SubcategoriaId == int.Parse(subcategorias));

        if (activeFilter != "all" && !string.IsNullOrEmpty(activeFilter))
        {
            var isActive = activeFilter == "true";
            query = query.Where(p => p.TB_Activo == isActive);
        }

        var productos = await query.ToListAsync();
        return View("Index", productos);
    }

    public async Task<IActionResult> CreateKardexEntry(int id)
    {
        var viewModel = new KardexViewModel
        {
            Fecha = DateTime.Now,
            ProductosDisponibles = await _context.TECO_A_Producto
                .Where(p => p.TB_Activo == true)
                .ToListAsync()
        };

        ViewData["TipoMovimientoId"] =
            new SelectList(
                await _context.TECO_M_TipoMovimientoKardex.Where(t => t.TB_Activo == true && t.TB_Entrada)
                    .ToListAsync(),
                "Id", "Tipo");

        ViewBag.Producto = await _context.TECO_A_Producto.FindAsync(id);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateKardexEntry(KardexViewModel viewModel)
    {
        if (ModelState.IsValid)
            try
            {
                var kardex = new TECO_P_Kardex
                {
                    TN_ProductoId = viewModel.ProductoId,
                    TF_Fecha = viewModel.Fecha,
                    TN_TipoMovimientoId = viewModel.TipoMovimientoId,
                    TN_Cantidad = viewModel.Cantidad,
                    TC_Descripcion = viewModel.Descripcion,
                    TB_Activo = viewModel.Activo
                };

                if (kardex.TN_ProductoId.HasValue)
                {
                    var producto = await _context.TECO_A_Producto.FindAsync(kardex.TN_ProductoId);
                    if (producto != null)
                    {
                        kardex.TN_StockAnterior = producto.TN_Stock;
                        var tipoMovimiento =
                            await _context.TECO_M_TipoMovimientoKardex.FindAsync(kardex.TN_TipoMovimientoId);
                        if (tipoMovimiento != null && tipoMovimiento.TB_Entrada)
                        {
                            producto.TN_Stock += kardex.TN_Cantidad ?? 0;
                            kardex.TN_StockActual = producto.TN_Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Kardex", new { id = kardex.TN_Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el movimiento de kardex: " + ex.Message);
            }

        viewModel.ProductosDisponibles = await _context.TECO_A_Producto
            .Where(p => p.TB_Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new SelectList(
            await _context.TECO_M_TipoMovimientoKardex.Where(t => t.TB_Activo == true && t.TB_Entrada).ToListAsync(),
            "Id", "Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    public async Task<IActionResult> CreateKardexExit(int id)
    {
        var viewModel = new KardexViewModel
        {
            Fecha = DateTime.Now,
            ProductosDisponibles = await _context.TECO_A_Producto
                .Where(p => p.TB_Activo == true)
                .ToListAsync()
        };

        ViewData["TipoMovimientoId"] =
            new SelectList(
                await _context.TECO_M_TipoMovimientoKardex.Where(t => t.TB_Activo == true && !t.TB_Entrada)
                    .ToListAsync(),
                "Id", "Tipo");

        ViewBag.Producto = await _context.TECO_A_Producto.FindAsync(id);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateKardexExit(KardexViewModel viewModel)
    {
        if (ModelState.IsValid)
            try
            {
                var kardex = new TECO_P_Kardex
                {
                    TN_ProductoId = viewModel.ProductoId,
                    TF_Fecha = viewModel.Fecha,
                    TN_TipoMovimientoId = viewModel.TipoMovimientoId,
                    TN_Cantidad = viewModel.Cantidad,
                    TC_Descripcion = viewModel.Descripcion,
                    TB_Activo = viewModel.Activo
                };

                if (kardex.TN_ProductoId.HasValue)
                {
                    var producto = await _context.TECO_A_Producto.FindAsync(kardex.TN_ProductoId);
                    if (producto != null)
                    {
                        kardex.TN_StockAnterior = producto.TN_Stock;
                        var tipoMovimiento =
                            await _context.TECO_M_TipoMovimientoKardex.FindAsync(kardex.TN_TipoMovimientoId);
                        if (tipoMovimiento != null && !tipoMovimiento.TB_Entrada)
                        {
                            producto.TN_Stock += kardex.TN_Cantidad ?? 0;
                            kardex.TN_StockActual = producto.TN_Stock;
                            _context.Update(producto);
                        }
                    }
                }

                _context.Add(kardex);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Kardex", new { id = kardex.TN_Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el movimiento de kardex: " + ex.Message);
            }

        viewModel.ProductosDisponibles = await _context.TECO_A_Producto
            .Where(p => p.TB_Activo == true)
            .ToListAsync();

        ViewData["TipoMovimientoId"] = new SelectList(
            await _context.TECO_M_TipoMovimientoKardex.Where(t => t.TB_Activo == true && !t.TB_Entrada).ToListAsync(),
            "Id", "Tipo", viewModel.TipoMovimientoId);

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductoStock(int id)
    {
        var stock = await _context.TECO_A_Producto
            .Where(p => p.TN_Id == id)
            .Select(p => new { stock = p.TN_Stock })
            .FirstOrDefaultAsync();

        if (stock == null) return NotFound();
        return Json(stock);
    }

    [HttpGet]
    public async Task<IActionResult> GetTipoMovimiento(int id)
    {
        var tipoMovimiento = await _context.TECO_M_TipoMovimientoKardex
            .Where(t => t.TN_Id == id)
            .Select(t => new { esEntrada = t.TB_Entrada })
            .FirstOrDefaultAsync();

        if (tipoMovimiento == null) return NotFound();
        return Json(tipoMovimiento);
    }

    public async Task<IActionResult> ListaProductos(ProductListViewModel filterModel, int categoriaId = 0,
        int subcategoriaId = 0)
    {
        try
        {
            //Persistencia en para los ID de navegación de categoría y subcategoría
            Console.WriteLine("Categoría: " + categoriaId);
            Console.WriteLine("Subcategoría: " + subcategoriaId);
            filterModel.CurrentCategoriaId = categoriaId;
            filterModel.CurrentSubcategoriaId = subcategoriaId;

            //Inicializar la consulta con la DB mostrando solo productos activos
            var query = _context.TECO_A_Producto
                .Include(p => p.Marca)
                .Include(p => p.Subcategoria)
                .ThenInclude(s => s.Categoria)
                .Where(p => p.TB_Activo);

            // Aplicar filtros básicos de búsqueda
            if (categoriaId != 0)
            {
                query = query.Where(p => p.Subcategoria.TN_CategoriaId == categoriaId);
                ViewBag.Title = await _context.TECO_M_Categoria
                    .Where(c => c.TN_Id == categoriaId)
                    .Select(c => c.TC_Nombre)
                    .FirstOrDefaultAsync() ?? "Productos";
            }

            if (subcategoriaId != 0)
            {
                query = query.Where(p => p.TN_SubcategoriaId == subcategoriaId);
                ViewBag.Title = await _context.TECO_M_Subcategoria
                    .Where(s => s.TN_Id == subcategoriaId)
                    .Select(s => s.TC_Nombre)
                    .FirstOrDefaultAsync() ?? "Productos";
            }

            // Aplicar filtro de búsqueda global por termino (Desde el layout)
            if (!string.IsNullOrEmpty(filterModel.SearchTerm))
            {
                query = query.Where(p => p.TC_Nombre.ToLower().Contains(filterModel.SearchTerm.ToLower())
                                         || p.TC_Descripcion.ToLower().Contains(filterModel.SearchTerm.ToLower()));
                ViewBag.CurrentSearch = filterModel.SearchTerm;
            }
            else
            {
                ViewBag.CurrentSearch = string.Empty;
            }

            // Filtrar por categorías y subcategorías seleccionadas
            // Filtrar por Marcas seleccionadas
            if (filterModel.SelectedMarcaIds.Any())
                query = query.Where(p => filterModel.SelectedMarcaIds.Contains(p.Marca.TN_Id));


            // Filtrar por Categorías seleccionadas (desde el sidebar)
            if (filterModel.SelectedCategoriaIds.Any())
                query = query.Where(p =>
                    p.Subcategoria != null &&
                    filterModel.SelectedCategoriaIds.Contains(p.Subcategoria.Categoria.TN_Id));


            // Filtrar por Subcategorías seleccionadas (desde el sidebar)
            if (filterModel.SelectedSubcategoryIds.Any())
                query = query.Where(p =>
                    p.Subcategoria != null &&
                    filterModel.SelectedSubcategoryIds.Contains(p.Subcategoria.TN_Id));

            // Aplicar ordenamiento
            if (string.IsNullOrEmpty(filterModel.OrderBy)) filterModel.OrderBy = "name";
            if (string.IsNullOrEmpty(filterModel.OrderForm)) filterModel.OrderForm = "asc";

            switch (filterModel.OrderBy)
            {
                case "name":
                    query = filterModel.OrderForm == "asc"
                        ? query.OrderBy(p => p.TC_Nombre)
                        : query.OrderByDescending(p => p.TC_Nombre);
                    break;
                case "price":
                    query = filterModel.OrderForm == "asc"
                        ? query.OrderBy(p => p.TN_Precio)
                        : query.OrderByDescending(p => p.TN_Precio);
                    break;
                default:
                    query = query.OrderBy(p => p.TC_Nombre);
                    break;
            }

            // Obtener la lista de productos filtrados
            filterModel.ProductosList = await query.ToListAsync();
            var marcasIds = filterModel.ProductosList.Select(p => p.TN_MarcaId).Distinct().ToList();
            filterModel.ProductosMarcas = await _context.TECO_M_Marca
                .Where(m => marcasIds.Contains(m.TN_Id) && m.TB_Activo)
                .ToListAsync();
            var categoriasIds = filterModel.ProductosList
                .Select(p => p.Subcategoria.TN_CategoriaId)
                .Distinct()
                .ToList();
            filterModel.ProductosCategorias = await _context.TECO_M_Categoria
                .Where(c => categoriasIds.Contains(c.TN_Id) && c.TB_Activo)
                .ToListAsync();

            // Las subcategorías deben mostrarse según la categoría seleccionada si hay una categoríaId inicial
            // O si se ha seleccionado alguna categoría en los filtros del sidebar.
            if (categoriaId > 0)
                filterModel.ProductosSubcategorias = await _context.TECO_M_Subcategoria
                    .Where(s => s.TN_CategoriaId == categoriaId && s.TB_Activo)
                    .ToListAsync();
            else if (filterModel.SelectedCategoriaIds.Any()) // Si se filtró por categorías en el sidebar
                filterModel.ProductosSubcategorias = await _context.TECO_M_Subcategoria
                    .Where(s => filterModel.SelectedCategoriaIds.Contains(s.Categoria.TN_Id) && s.TB_Activo)
                    .ToListAsync();
            else // Si no hay filtro de categoría, mostrar todas las subcategorías activas
                filterModel.ProductosSubcategorias =
                    await _context.TECO_M_Subcategoria.Where(s => s.TB_Activo).ToListAsync();

            //Manejar mensajes si no hay productos
            if (!filterModel.ProductosList.Any())
                TempData["info"] = System.Text.Json.JsonSerializer.Serialize(
                    Alert.InfoAlert("No se encontraron productos con los filtros aplicados"));
            else
                ViewBag.Message = null; // Limpiar el mensaje si hay productos

            return View(filterModel);
        }
        catch (Exception ex)
        {
            TempData["Alert"] = System.Text.Json.JsonSerializer.Serialize(
                Alert.ErrorAlert("Error al cargar la lista de productos"));
            _logger.LogError(ex, "Error al cargar la lista de productos");
            return RedirectToAction("Index", "Home");
        }
    }
}