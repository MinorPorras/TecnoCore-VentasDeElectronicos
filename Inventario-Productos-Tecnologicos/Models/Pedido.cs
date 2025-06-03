using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class Pedido
{
    [Key]
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public int? MetodoPagoId { get; set; }

    public int? EstadoPedidoId { get; set; }

    [StringLength(255)]
    public string? TransaccionId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Fecha { get; set; }

    public int? CuponId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Subtotal { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Impuesto { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Descuento { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Total { get; set; }

    public bool? Activo { get; set; }

    [ForeignKey("CuponId")]
    [InverseProperty("Pedidos")]
    public virtual Cupone? Cupon { get; set; }

    [InverseProperty("Pedido")]
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    [ForeignKey("EstadoPedidoId")]
    [InverseProperty("Pedidos")]
    public virtual EstadosPedido? EstadoPedido { get; set; }

    [ForeignKey("MetodoPagoId")]
    [InverseProperty("Pedidos")]
    public virtual MetodosPago? MetodoPago { get; set; }

    [ForeignKey("UsuarioId")]
    [InverseProperty("Pedidos")]
    public virtual Usuario? Usuario { get; set; }
}
