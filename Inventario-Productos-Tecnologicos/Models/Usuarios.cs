using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un usuario del sistema de inventario.
/// </summary>
public partial class Usuarios
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Correo electrónico del usuario.
    /// Máximo 100 caracteres.
    /// </summary>
    [StringLength(100)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// Nombre del usuario.
    /// Máximo 30 caracteres.
    /// </summary>
    [StringLength(30)]
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Apellidos del usuario.
    /// Máximo 50 caracteres.
    /// </summary>
    [StringLength(50)]
    public string Apellidos { get; set; } = null!;

    /// <summary>
    /// Contraseña del usuario.
    /// Máximo 255 caracteres.
    /// </summary>
    [StringLength(255)]
    public string Contrasena { get; set; } = null!;

    /// <summary>
    /// Número de teléfono del usuario.
    /// Máximo 20 caracteres.
    /// </summary>
    [StringLength(20)]
    public string? Telefono { get; set; }

    /// <summary>
    /// Identificador del rol asignado al usuario.
    /// </summary>
    public int? Rol { get; set; }

    /// <summary>
    /// Indica si el usuario está activo en el sistema.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Colección de direcciones asociadas al usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual Direcciones? Direccion { get; set; }

    /// <summary>
    /// Colección de listas de deseos del usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual ICollection<ListaDeseos> ListaDeseos { get; set; } = new List<ListaDeseos>();

    /// <summary>
    /// Colección de pedidos realizados por el usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual ICollection<Pedidos> Pedidos { get; set; } = new List<Pedidos>();

    /// <summary>
    /// Referencia al rol asignado al usuario.
    /// </summary>
    [ForeignKey("Rol")]
    [InverseProperty("Usuarios")]
    public virtual Roles? RolNavigation { get; set; }
}