using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un rol de usuario en el sistema de inventario.
/// </summary>
public partial class Role
{
    /// <summary>
    /// Identificador único del rol.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nombre del rol.
    /// Máximo 50 caracteres.
    /// </summary>
    [StringLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Indica si el rol está activo en el sistema.
    /// </summary>
    public bool? Activo { get; set; }

    /// <summary>
    /// Colección de usuarios que tienen asignado este rol.
    /// </summary>
    [InverseProperty("RolNavigation")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}