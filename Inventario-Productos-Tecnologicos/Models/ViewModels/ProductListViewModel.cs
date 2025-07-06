// Models/ViewModels/ProductListViewModel.cs

namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class ProductListViewModel
{
    private List<Productos>? _productosList;
    private List<Marcas>? _productosMarcas;
    private List<Categorias>? _productosCategorias;
    private List<Subcategorias>? _productosSubcategorias;

    public List<Productos> ProductosList
    {
        get => _productosList ??= new List<Productos>();
        set => _productosList = value;
    }

    public List<Marcas> ProductosMarcas
    {
        get => _productosMarcas ??= new List<Marcas>();
        set => _productosMarcas = value;
    }

    public List<Categorias>? ProductosCategorias
    {
        get => _productosCategorias ??= new List<Categorias>();
        set => _productosCategorias = value;
    }

    public List<Subcategorias>? ProductosSubcategorias
    {
        get => _productosSubcategorias ??= new List<Subcategorias>();
        set => _productosSubcategorias = value;
    }

    // --- PROPIEDADES PARA FILTROS (actualizadas) ---

    // Para el término de búsqueda global
    public string? SearchTerm { get; set; }

    // Para los checkboxes de Categorías seleccionadas (si aplica en la vista de filtros)
    public List<int> SelectedCategoriaIds { get; set; } = new();

    // Para los checkboxes de Subcategorías seleccionadas
    public List<int> SelectedSubcategoryIds { get; set; } = new();

    // Para los checkboxes de Marcas seleccionadas
    public List<int> SelectedMarcaIds { get; set; } = new();

    // Para el radio button de orden (nombre/precio)
    public string? OrderBy { get; set; }

    // Para el radio button de forma de orden (ascendente/descendente)
    public string? OrderForm { get; set; } // "asc" o "desc"

    // Propiedades para mantener el estado de los filtros iniciales de navegación
    public int CurrentCategoriaId { get; set; }
    public int CurrentSubcategoriaId { get; set; }
}