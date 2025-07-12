using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa el detalle de un pedido, incluyendo los productos y sus cantidades.
/// </summary>
public partial class TECO_P_DetallePedido
{
    /// <summary>
    /// Identificador único del detalle de pedido.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Identificador del pedido al que pertenece este detalle.
    /// </summary>
    public int? TN_PedidoId { get; set; }

    /// <summary>
    /// Identificador del producto incluido en este detalle.
    /// </summary>
    public int? TN_ProductoId { get; set; }

    /// <summary>
    /// Cantidad del producto solicitada en el pedido.
    /// </summary>
    public int? TN_Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del producto al momento de realizar el pedido.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TN_PrecioUnitario { get; set; }

    /// <summary>
    /// Indica si el detalle del pedido está activo.
    /// </summary>
    public bool? TB_Activo { get; set; }

    /// <summary>
    /// Referencia al pedido al que pertenece este detalle.
    /// </summary>
    [ForeignKey("TN_PedidoId")]
    [InverseProperty("DetallePedidos")]
    public virtual TECO_P_Pedido? Pedido { get; set; }

    /// <summary>
    /// Referencia al producto incluido en este detalle.
    /// </summary>
    [ForeignKey("TN_ProductoId")]
    [InverseProperty("DetallePedidos")]
    public virtual TECO_A_Producto? Producto { get; set; }
}