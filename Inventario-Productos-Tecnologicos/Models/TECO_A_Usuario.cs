using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un usuario del sistema de inventario.
/// </summary>
public class TECO_A_Usuario : IdentityUser
{
    /// <summary>
    /// Nombre del usuario.
    /// Máximo 30 caracteres.
    /// </summary>
    [StringLength(30)]
    public string TC_Nombre { get; set; } = null!;

    /// <summary>
    /// Apellidos del usuario.
    /// Máximo 50 caracteres.
    /// </summary>
    [StringLength(50)]
    public string TC_Apellidos { get; set; } = null!;

    /// <summary>
    /// Indica si el usuario está activo en el sistema.
    /// </summary>
    public bool TB_Activo { get; set; }

    /// <summary>
    /// Direccion asociadas al usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual TECO_A_Direccion? Direccion { get; set; }

    /// <summary>
    /// Colección de listas de deseos del usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual ICollection<TECO_P_ListaDeseos> ListaDeseos { get; set; } = new List<TECO_P_ListaDeseos>();

    /// <summary>
    /// Colección de pedidos realizados por el usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual ICollection<TECO_P_Pedido> Pedido { get; set; } = new List<TECO_P_Pedido>();

    /// <summary>
    /// Colección de artículos en el carrito de compras del usuario.
    /// </summary>
    [InverseProperty("Usuario")]
    public virtual ICollection<TECO_P_CarritoCompras> CarritoCompras { get; set; } = new List<TECO_P_CarritoCompras>();
}