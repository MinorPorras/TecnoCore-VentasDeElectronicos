using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class Categoria
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public bool? Activo { get; set; }

    [InverseProperty("Categoria")]
    public virtual ICollection<Subcategoria> Subcategoria { get; set; } = new List<Subcategoria>();
}
