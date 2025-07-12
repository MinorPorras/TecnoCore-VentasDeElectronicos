using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa una categoría de productos en el sistema.
/// </summary>
public partial class TECO_M_Categoria
{
    /// <summary>
    /// Identificador único de la categoría.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Nombre de la categoría.
    /// Máximo 100 caracteres.
    /// </summary>
    [StringLength(100)]
    public string TC_Nombre { get; set; } = null!;

    /// <summary>
    /// Indica si la categoría está activa en el sistema.
    /// </summary>
    public bool TB_Activo { get; set; }

    /// <summary>
    /// Colección de subcategorías pertenecientes a esta categoría.
    /// </summary>
    [InverseProperty("Categoria")]
    public virtual ICollection<TECO_M_Subcategoria> Subcategoria { get; set; } = new List<TECO_M_Subcategoria>();
}