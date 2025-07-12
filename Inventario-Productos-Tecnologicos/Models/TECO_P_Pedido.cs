using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Representa un pedido realizado en el sistema de inventario de productos tecnológicos.
/// </summary>
public partial class TECO_P_Pedido
{
    /// <summary>
    /// Identificador único del pedido.
    /// </summary>
    [Key]
    public int TN_Id { get; set; }

    /// <summary>
    /// Identificador del usuario que realizó el pedido.
    /// </summary>
    public string? TN_UsuarioId { get; set; }

    /// <summary>
    /// Identificador del método de pago utilizado.
    /// </summary>
    public int? TN_MetodoPagoId { get; set; }

    [StringLength(16)] public string TC_NumTarjeta { get; set; } = null!;

    /// <summary>
    /// Identificador del estado actual del pedido.
    /// </summary>
    public int? TN_EstadoPedidoId { get; set; }

    /// <summary>
    /// Identificador único de la transacción de pago.
    /// Máximo 255 caracteres.
    /// </summary>
    [StringLength(255)]
    public string? TN_TransaccionId { get; set; }

    /// <summary>
    /// Fecha y hora en que se realizó el pedido.
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? TF_Fecha { get; set; }

    /// <summary>
    /// Identificador del cupón de descuento aplicado al pedido.
    /// </summary>
    public int? TN_CuponId { get; set; }

    /// <summary>
    /// Monto subtotal del pedido antes de impuestos y descuentos.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TN_Subtotal { get; set; }

    /// <summary>
    /// Monto del impuesto aplicado al pedido.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TN_Impuesto { get; set; }

    /// <summary>
    /// Monto del descuento aplicado al pedido.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TN_Descuento { get; set; }

    /// <summary>
    /// Monto total del pedido incluyendo impuestos y descuentos.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TN_Total { get; set; }

    /// <summary>
    /// Indica si el pedido está activo en el sistema.
    /// </summary>
    public bool? TB_Activo { get; set; }

    /// <summary>
    /// Referencia al cupón de descuento aplicado al pedido.
    /// </summary>
    [ForeignKey("CuponId")]
    [InverseProperty("Pedido")]
    public virtual TECO_M_Cupon? Cupon { get; set; }

    /// <summary>
    /// Colección de detalles que componen el pedido.
    /// </summary>
    [InverseProperty("Pedido")]
    public virtual ICollection<TECO_P_DetallePedido> DetallePedidos { get; set; } = new List<TECO_P_DetallePedido>();

    /// <summary>
    /// Referencia al estado actual del pedido.
    /// </summary>
    [ForeignKey("TN_EstadoPedidoId")]
    [InverseProperty("Pedido")]
    public virtual TECO_M_EstadoPedido? EstadoPedido { get; set; }

    /// <summary>
    /// Referencia al método de pago utilizado.
    /// </summary>
    [ForeignKey("TN_MetodoPagoId")]
    [InverseProperty("Pedido")]
    public virtual TECO_M_MetodoPago? MetodoPago { get; set; }

    /// <summary>
    /// Referencia al usuario que realizó el pedido.
    /// </summary>
    [ForeignKey("TN_UsuarioId")]
    [InverseProperty("Pedido")]
    public virtual TECO_A_Usuario? Usuario { get; set; }
}