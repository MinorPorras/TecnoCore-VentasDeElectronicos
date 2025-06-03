using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

[Index("Codigo", Name = "UQ__Cupones__06370DACEA3BF6E0", IsUnique = true)]
public partial class Cupone
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    [StringLength(200)]
    public string? Descripcion { get; set; }

    [StringLength(50)]
    public string TipoDescuento { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Valor { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    public int? UsosMaximos { get; set; }

    public int? UsosActuales { get; set; }

    public bool? Activo { get; set; }

    [InverseProperty("Cupon")]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
