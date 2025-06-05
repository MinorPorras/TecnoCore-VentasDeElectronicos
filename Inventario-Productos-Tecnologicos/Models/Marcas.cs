using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa una marca de productos tecnológicos.
/// </summary>
public partial class Marcas
{
    /// <summary>
    /// Identificador único de la marca.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la marca.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;
    

    /// <summary>
    /// Indica si la marca está activa en el sistema.
    /// </summary>
    public bool Activo { get; set; } = true;
    

    /// <summary>
    /// Colección de productos asociados a esta marca.
    /// </summary>
    [InverseProperty("Marca")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}