using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

[Table("EstadosPedido")]
public partial class EstadosPedido
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string NombreEstado { get; set; } = null!;

    public bool? Activo { get; set; }

    [InverseProperty("EstadoPedido")]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
