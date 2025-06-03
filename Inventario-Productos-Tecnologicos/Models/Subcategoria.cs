using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class Subcategoria
{
    [Key]
    public int Id { get; set; }

    [Column("NOMBRE")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("CategoriaID")]
    public int? CategoriaId { get; set; }

    public bool? Activo { get; set; }

    [ForeignKey("CategoriaId")]
    [InverseProperty("Subcategoria")]
    public virtual Categoria? Categoria { get; set; }

    [InverseProperty("Subcategoria")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
