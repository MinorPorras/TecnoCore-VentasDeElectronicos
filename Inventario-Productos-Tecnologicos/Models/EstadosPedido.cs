using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa los posibles estados que puede tener un pedido en el sistema.
/// </summary>
[Table("EstadosPedido")]
public partial class EstadosPedido
{
    /// <summary>
    /// Identificador único del estado de pedido.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nombre descriptivo del estado del pedido.
    /// Máximo 50 caracteres.
    /// </summary>
    [StringLength(50)]
    public string NombreEstado { get; set; } = null!;

    /// <summary>
    /// Indica si el estado de pedido está activo en el sistema.
    /// </summary>
    public bool? Activo { get; set; }

    /// <summary>
    /// Colección de pedidos que se encuentran en este estado.
    /// </summary>
    [InverseProperty("EstadoPedido")]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}