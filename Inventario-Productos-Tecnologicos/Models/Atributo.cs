using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class Atributo
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string NombreAtributo { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public bool? Activo { get; set; }

    [InverseProperty("Atributo")]
    public virtual ICollection<ProductoAtributo> ProductoAtributos { get; set; } = new List<ProductoAtributo>();
}
