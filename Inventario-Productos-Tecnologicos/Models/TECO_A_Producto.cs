using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un producto en el inventario del sistema.
/// </summary>
public partial class TECO_A_Producto
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Nombre del producto.
    /// Máximo 100 caracteres.
    /// </summary>
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string TC_Nombre { get; set; } = null!;

    /// <summary>
    /// Descripción detallada del producto.
    /// Máximo 300 caracteres.
    /// </summary>
    [StringLength(300)]
    [Display(Name = "Descripción")]
    public string? TC_Descripcion { get; set; }

    /// <summary>
    /// Precio unitario del producto.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    [Display(Name = "Precio")]
    public decimal TN_Precio { get; set; }

    /// <summary>
    /// Cantidad disponible en inventario.
    /// </summary>
    [Display(Name = "Stock")]
    public int TN_Stock { get; set; }

    /// <summary>
    /// Ruta de la imagen del producto.
    /// Máximo 255 caracteres.
    /// </summary>
    [StringLength(255)]
    [Display(Name = "Imagen")]
    public string? TC_Imagen { get; set; }

    /// <summary>
    /// Indica si el producto es una novedad en el catálogo.
    /// </summary>
    [Display(Name = "Novedad")]
    public bool TB_Novedad { get; set; } = false;

    [Display(Name = "Marca")] public int? TN_MarcaId { get; set; }

    /// <summary>
    /// Identificador de la subcategoría a la que pertenece el producto.
    /// </summary>
    [Display(Name = "Subcategoría")]
    public int? TN_SubcategoriaId { get; set; }

    /// <summary>
    /// Indica si el producto está activo en el sistema.
    /// </summary>
    [Display(Name = "Activo")]
    public bool TB_Activo { get; set; }

    /// <summary>
    /// Colección de detalles de pedidos que incluyen este producto.
    /// </summary>
    [InverseProperty("Producto")]
    public virtual ICollection<TECO_P_DetallePedido> DetallePedidos { get; set; } = new List<TECO_P_DetallePedido>();

    /// <summary>
    /// Colección de registros de kardex asociados a este producto.
    /// </summary>
    [InverseProperty("Producto")]
    public virtual ICollection<TECO_P_Kardex> Kardex { get; set; } = new List<TECO_P_Kardex>();

    /// <summary>
    /// Colección de listas de deseos que incluyen este producto.
    /// </summary>
    [InverseProperty("Producto")]
    public virtual ICollection<TECO_P_ListaDeseos> ListaDeseos { get; set; } = new List<TECO_P_ListaDeseos>();

    [ForeignKey("TN_MarcaId")]
    [InverseProperty("Productos")]
    public virtual TECO_M_Marca? Marca { get; set; }

    /// <summary>
    /// Referencia a la subcategoría a la que pertenece el producto.
    /// </summary>
    [ForeignKey("TN_SubcategoriaId")]
    [InverseProperty("Productos")]
    public virtual TECO_M_Subcategoria? Subcategoria { get; set; }

    /// <summary>
    /// Colección de registros de carrito de compras que incluyen este producto.
    /// </summary>
    [InverseProperty("Producto")]
    public virtual ICollection<TECO_P_CarritoCompras> CarritoCompras { get; set; } = new List<TECO_P_CarritoCompras>();
}