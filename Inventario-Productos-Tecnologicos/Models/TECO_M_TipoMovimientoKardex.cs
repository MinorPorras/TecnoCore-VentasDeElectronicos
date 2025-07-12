using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un tipo de movimiento en el kardex del inventario.
/// </summary>
public partial class TECO_M_TipoMovimientoKardex
{
    /// <summary>
    /// Identificador único del tipo de movimiento.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Nombre o descripción del tipo de movimiento.
    /// Máximo 50 caracteres.
    /// </summary>
    [StringLength(50)]
    public string TC_Tipo { get; set; } = null!;

    public bool TB_Entrada { get; set; }

    /// <summary>
    /// Indica si el tipo de movimiento está activo en el sistema.
    /// </summary>
    public bool? TB_Activo { get; set; }

    /// <summary>
    /// Colección de registros de kardex asociados a este tipo de movimiento.
    /// </summary>
    [InverseProperty("TipoMovimientoKardex")]
    public virtual ICollection<TECO_P_Kardex> Kardex { get; set; } = new List<TECO_P_Kardex>();
}