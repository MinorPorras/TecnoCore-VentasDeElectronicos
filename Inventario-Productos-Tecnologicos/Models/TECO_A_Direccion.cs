using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa una dirección de envío asociada a un usuario.
/// </summary>
public class TECO_A_Direccion
{
    /// <summary>
    /// Identificador único de la dirección.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Identificador del usuario al que pertenece esta dirección.
    /// </summary>
    public string? TN_UsuarioId { get; set; }

    /// <summary>
    /// Nombre de la calle y número.
    /// Máximo 200 caracteres.
    /// </summary>
    [Required(ErrorMessage = "La dirección exacta es obligatoria.")]
    [StringLength(200)]
    public string TC_Direccion { get; set; } = null!;

    /// <summary>
    /// Ciudad donde se encuentra la dirección.
    /// Máximo 100 caracteres.
    /// </summary>
    [Required(ErrorMessage = "El cantón es obligatorio para la dirección.")]
    public int TN_CantonId { get; set; }

    /// <summary>
    /// Código postal de la dirección.
    /// Máximo 20 caracteres.
    /// </summary>
    [Required(ErrorMessage = "El código postal es obligatorio.")]
    [StringLength(20)]
    public string TC_CodigoPostal { get; set; } = null!;

    /// <summary>
    /// Indica si la dirección está activa en el sistema.
    /// </summary>
    public bool? TB_Activo { get; set; }

    /// <summary>
    /// Referencia al usuario propietario de esta dirección.
    /// </summary>
    [ForeignKey("TN_UsuarioId")]
    [InverseProperty("Direccion")]
    public virtual TECO_A_Usuario? Usuario { get; set; }

    /// <summary>
    /// Propiedad de navegación al Cantón
    /// </summary>
    [ForeignKey("TN_CantonId")]
    [InverseProperty("Direccion")]
    public virtual TECO_M_Canton? Canton { get; set; }
}