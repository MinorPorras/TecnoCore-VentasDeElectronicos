using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un elemento de la lista de deseos de un usuario.
/// </summary>
public partial class TECO_P_ListaDeseos
{
    /// <summary>
    /// Identificador único del elemento en la lista de deseos.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Identificador del usuario propietario de esta lista de deseos.
    /// </summary>
    public string? TN_UsuarioId { get; set; }

    /// <summary>
    /// Identificador del producto agregado a la lista de deseos.
    /// </summary>
    public int? TN_ProductoId { get; set; }

    /// <summary>
    /// Fecha y hora en que se agregó el producto a la lista de deseos.
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? TF_FechaAgregado { get; set; }

    /// <summary>
    /// Indica si el elemento de la lista de deseos está activo en el sistema.
    /// </summary>
    public bool? TB_Activo { get; set; }

    /// <summary>
    /// Referencia al producto agregado a la lista de deseos.
    /// </summary>
    [ForeignKey("TN_ProductoId")]
    [InverseProperty("ListaDeseos")]
    public virtual TECO_A_Producto? Producto { get; set; }

    /// <summary>
    /// Referencia al usuario propietario de esta lista de deseos.
    /// </summary>
    [ForeignKey("TN_UsuarioId")]
    [InverseProperty("ListaDeseos")]
    public virtual TECO_A_Usuario? Usuario { get; set; }
}