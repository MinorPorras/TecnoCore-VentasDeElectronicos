using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa una subcategoría de productos en el sistema de inventario.
/// </summary>
public partial class TECO_M_Subcategoria
{
    /// <summary>
    /// Identificador único de la subcategoría.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Nombre de la subcategoría.
    /// Máximo 100 caracteres.
    /// </summary>
    [StringLength(100)]
    public string TC_Nombre { get; set; } = null!;

    /// <summary>
    /// Identificador de la categoría principal a la que pertenece esta subcategoría.
    /// </summary>
    public int TN_CategoriaId { get; set; }

    /// <summary>
    /// Indica si la subcategoría está activa en el sistema.
    /// </summary>
    public bool TB_Activo { get; set; }

    /// <summary>
    /// Referencia a la categoría principal a la que pertenece esta subcategoría.
    /// </summary>
    [ForeignKey("TN_CategoriaId")]
    [InverseProperty("Subcategoria")]
    public virtual TECO_M_Categoria? Categoria { get; set; }

    /// <summary>
    /// Colección de productos que pertenecen a esta subcategoría.
    /// </summary>
    [InverseProperty("Subcategoria")]
    public virtual ICollection<TECO_A_Producto> Productos { get; set; } = new List<TECO_A_Producto>();
}