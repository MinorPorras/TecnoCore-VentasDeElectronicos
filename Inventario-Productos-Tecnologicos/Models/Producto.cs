using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class Producto
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Precio { get; set; }

    public int? Stock { get; set; }

    [StringLength(255)]
    public string? Imagen { get; set; }

    public int? SubcategoriaId { get; set; }

    public bool? Activo { get; set; }

    [InverseProperty("Producto")]
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    [InverseProperty("Producto")]
    public virtual ICollection<Kardex> Kardices { get; set; } = new List<Kardex>();

    [InverseProperty("Producto")]
    public virtual ICollection<ListaDeseo> ListaDeseos { get; set; } = new List<ListaDeseo>();

    [InverseProperty("Producto")]
    public virtual ICollection<ProductoAtributo> ProductoAtributos { get; set; } = new List<ProductoAtributo>();

    [ForeignKey("SubcategoriaId")]
    [InverseProperty("Productos")]
    public virtual Subcategoria? Subcategoria { get; set; }
}
