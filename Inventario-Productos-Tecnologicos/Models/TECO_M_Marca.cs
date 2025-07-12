using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa una marca de productos tecnológicos.
/// </summary>
public partial class TECO_M_Marca
{
    /// <summary>
    /// Identificador único de la marca.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Nombre de la marca.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string TC_Nombre { get; set; } = null!;


    /// <summary>
    /// Indica si la marca está activa en el sistema.
    /// </summary>
    public bool TB_Activo { get; set; } = true;


    /// <summary>
    /// Colección de productos asociados a esta marca.
    /// </summary>
    [InverseProperty("Marca")]
    public virtual ICollection<TECO_A_Producto> Productos { get; set; } = new List<TECO_A_Producto>();
}