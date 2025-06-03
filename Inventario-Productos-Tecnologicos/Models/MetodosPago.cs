using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

[Table("MetodosPago")]
public partial class MetodosPago
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string NombreMetodo { get; set; } = null!;

    public bool? Activo { get; set; }

    [InverseProperty("MetodoPago")]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
