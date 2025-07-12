// Models/ViewModels/ProductListViewModel.cs

namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class ProductListViewModel
{
    private List<TECO_A_Producto>? _productosList;
    private List<TECO_M_Marca>? _productosMarcas;
    private List<TECO_M_Categoria>? _productosCategorias;
    private List<TECO_M_Subcategoria>? _productosSubcategorias;

    public List<TECO_A_Producto> ProductosList
    {
        get => _productosList ??= new List<TECO_A_Producto>();
        set => _productosList = value;
    }

    public List<TECO_M_Marca> ProductosMarcas
    {
        get => _productosMarcas ??= new List<TECO_M_Marca>();
        set => _productosMarcas = value;
    }

    public List<TECO_M_Categoria>? ProductosCategorias
    {
        get => _productosCategorias ??= new List<TECO_M_Categoria>();
        set => _productosCategorias = value;
    }

    public List<TECO_M_Subcategoria>? ProductosSubcategorias
    {
        get => _productosSubcategorias ??= new List<TECO_M_Subcategoria>();
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