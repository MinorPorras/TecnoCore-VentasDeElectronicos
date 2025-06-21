using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un cupón de descuento que puede ser aplicado a pedidos.
/// </summary>
[Index("Codigo", Name = "UQ__Cupones__06370DACEA3BF6E0", IsUnique = true)]
public partial class Cupones
{
    /// <summary>
    /// Identificador único del cupón.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Código único del cupón.
    /// Máximo 50 caracteres.
    /// </summary>
    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    /// <summary>
    /// Descripción detallada del cupón.
    /// Máximo 200 caracteres.
    /// </summary>
    [StringLength(200)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de descuento aplicado (ej: porcentaje, monto fijo).
    /// Máximo 50 caracteres.
    /// </summary>
    [StringLength(50)]
    public string TipoDescuento { get; set; } = null!;

    /// <summary>
    /// Valor del descuento según el tipo especificado.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Valor { get; set; }

    /// <summary>
    /// Fecha de inicio de validez del cupón.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Fecha de fin de validez del cupón.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Required(ErrorMessage = "La fecha de finalización es requerida")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de finalización")]
    [CustomValidation(typeof(Cupones), nameof(ValidarFechaFin))]
    public DateTime FechaFin { get; set; }

    /// <summary>
    /// Número máximo de veces que se puede usar el cupón.
    /// </summary>
    public int? UsosMaximos { get; set; }

    /// <summary>
    /// Número de veces que se ha usado el cupón.
    /// </summary>
    public int? UsosActuales { get; set; }

    /// <summary>
    /// Indica si el cupón está activo para su uso.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Colección de pedidos que han utilizado este cupón.
    /// </summary>
    [InverseProperty("Cupon")]
    public virtual ICollection<Pedidos> Pedidos { get; set; } = new List<Pedidos>();

    public static ValidationResult ValidarFechaFin(DateTime fechaFin, ValidationContext context)
    {
        var cupon = (Cupones)context.ObjectInstance;
        if (fechaFin < cupon.FechaInicio)
            return new ValidationResult("La fecha de finalización debe ser posterior a la fecha de inicio");
        return ValidationResult.Success;
    }
}