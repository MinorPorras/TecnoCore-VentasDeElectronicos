using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

[Table("TipoMovimientoKardex")]
public partial class TipoMovimientoKardex
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    public bool? Entrada { get; set; }

    public bool? Activo { get; set; }

    [InverseProperty("TipoMovimiento")]
    public virtual ICollection<Kardex> Kardices { get; set; } = new List<Kardex>();
}
