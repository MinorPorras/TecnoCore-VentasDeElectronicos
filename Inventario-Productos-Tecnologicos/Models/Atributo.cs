using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un atributo que puede estar asociado a productos.
/// </summary>
public partial class Atributo
{
    /// <summary>
    /// Identificador único del atributo.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nombre descriptivo del atributo.
    /// Máximo 100 caracteres.
    /// </summary>
    [StringLength(100)]
    public string NombreAtributo { get; set; } = null!;

    /// <summary>
    /// Indica si el atributo está activo en el sistema.
    /// </summary>
    
    [StringLength(100)]
    public string Valor { get; set; } = null!;

    /// <summary>
    /// Indica si el atributo está activo en el sistema.
    /// </summary>
    public bool? Activo { get; set; }

    /// <summary>
    /// Colección de relaciones entre productos y este atributo.
    /// </summary>
    [InverseProperty("Atributo")]
    public virtual ICollection<ProductoAtributo> ProductoAtributos { get; set; } = new List<ProductoAtributo>();
}
