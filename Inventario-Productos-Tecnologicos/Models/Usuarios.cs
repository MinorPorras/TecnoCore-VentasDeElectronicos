using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un usuario del sistema de inventario.
/// </summary>
public class Usuarios : IdentityUser
{
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
    /// Colección de artículos en el carrito de compras del usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual ICollection<CarritoCompras> CarritoCompras { get; set; } = new List<CarritoCompras>();
}