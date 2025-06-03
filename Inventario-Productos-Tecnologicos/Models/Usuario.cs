using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class Usuario
{
    [Key] public int Id { get; set; }

    [StringLength(100)] public string Email { get; set; } = null!;

    [StringLength(30)] public string Nombre { get; set; } = null!;

    [StringLength(50)] public string Apellidos { get; set; } = null!;

    [StringLength(255)] public string Contrasena { get; set; } = null!;

    [StringLength(20)] public string? Telefono { get; set; }

    public int? Rol { get; set; }

    public bool? Activo { get; set; }

    [InverseProperty("Usuario")]
    public virtual ICollection<Direccione> Direcciones { get; set; } = new List<Direccione>();

    [InverseProperty("Usuario")]
    public virtual ICollection<ListaDeseo> ListaDeseos { get; set; } = new List<ListaDeseo>();

    [InverseProperty("Usuario")] public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    [ForeignKey("Rol")]
    [InverseProperty("Usuarios")]
    public virtual Role? RolNavigation { get; set; }
}