using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class ListaDeseo
{
    [Key]
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public int? ProductoId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaAgregado { get; set; }

    public bool? Activo { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("ListaDeseos")]
    public virtual Producto? Producto { get; set; }

    [ForeignKey("UsuarioId")]
    [InverseProperty("ListaDeseos")]
    public virtual Usuario? Usuario { get; set; }
}
