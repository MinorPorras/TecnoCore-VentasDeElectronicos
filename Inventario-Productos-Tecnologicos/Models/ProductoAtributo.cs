using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class ProductoAtributo
{
    [Key]
    public int Id { get; set; }

    public int? ProductoId { get; set; }

    public int? AtributoId { get; set; }

    [StringLength(255)]
    public string Valor { get; set; } = null!;

    public bool? Activo { get; set; }

    [ForeignKey("AtributoId")]
    [InverseProperty("ProductoAtributos")]
    public virtual Atributo? Atributo { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("ProductoAtributos")]
    public virtual Producto? Producto { get; set; }
}
