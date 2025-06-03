using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

[Table("KARDEX")]
public partial class Kardex
{
    [Key]
    public int Id { get; set; }

    public int? ProductoId { get; set; }

    public int? Cantidad { get; set; }

    [StringLength(300)]
    public string? Descripcion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Fecha { get; set; }

    public int? StockAnterior { get; set; }

    public int? StockActual { get; set; }

    public int? TipoMovimientoId { get; set; }

    public bool? Activo { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("Kardices")]
    public virtual Producto? Producto { get; set; }

    [ForeignKey("TipoMovimientoId")]
    [InverseProperty("Kardices")]
    public virtual TipoMovimientoKardex? TipoMovimiento { get; set; }
}
