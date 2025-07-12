using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa los métodos de pago disponibles para los pedidos en el sistema.
/// </summary>
public partial class TECO_M_MetodoPago
{
    /// <summary>
    /// Identificador único del método de pago.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Nombre descriptivo del método de pago.
    /// Máximo 100 caracteres.
    /// </summary>
    [StringLength(100)]
    public string TB_NombreMetodo { get; set; } = null!;

    /// <summary>
    /// Indica si el método de pago está activo en el sistema.
    /// </summary>
    public bool? TB_Activo { get; set; }

    /// <summary>
    /// Colección de pedidos que utilizan este método de pago.
    /// </summary>
    [InverseProperty("MetodoPago")]
    public virtual ICollection<TECO_P_Pedido> Pedido { get; set; } = new List<TECO_P_Pedido>();
}