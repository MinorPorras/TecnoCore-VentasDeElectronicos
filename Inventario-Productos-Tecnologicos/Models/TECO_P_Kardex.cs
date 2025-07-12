using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa el registro de movimientos de inventario (kardex) de los productos.
/// </summary>
public partial class TECO_P_Kardex
{
    /// <summary>
    /// Identificador único del registro de kardex.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Identificador del producto al que pertenece este movimiento.
    /// </summary>
    public int? TN_ProductoId { get; set; }

    /// <summary>
    /// Cantidad de unidades involucradas en el movimiento.
    /// </summary>
    public int? TN_Cantidad { get; set; }

    /// <summary>
    /// Descripción detallada del movimiento realizado.
    /// Máximo 300 caracteres.
    /// </summary>
    [StringLength(300)]
    public string? TC_Descripcion { get; set; }

    /// <summary>
    /// Fecha y hora en que se realizó el movimiento.
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? TF_Fecha { get; set; }

    /// <summary>
    /// Cantidad en stock antes del movimiento.
    /// </summary>
    public int? TN_StockAnterior { get; set; }

    /// <summary>
    /// Cantidad en stock después del movimiento.
    /// </summary>
    public int? TN_StockActual { get; set; }

    /// <summary>
    /// Identificador del tipo de movimiento realizado.
    /// </summary>
    public int? TN_TipoMovimientoId { get; set; }

    /// <summary>
    /// Indica si el registro del kardex está activo en el sistema.
    /// </summary>
    public bool TB_Activo { get; set; } = true;

    /// <summary>
    /// Referencia al producto asociado a este movimiento.
    /// </summary>
    [ForeignKey("TN_ProductoId")]
    [InverseProperty("Kardex")]
    public virtual TECO_A_Producto? Producto { get; set; }

    /// <summary>
    /// Referencia al tipo de movimiento realizado.
    /// </summary>
    [ForeignKey("TN_TipoMovimientoId")]
    [InverseProperty("Kardex")]
    public virtual TECO_M_TipoMovimientoKardex? TipoMovimientoKardex { get; set; }
}