using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa un pedido realizado en el sistema de inventario de productos tecnológicos.
    /// </summary>
    public partial class Pedido
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Identificador del usuario que realizó el pedido.
        /// </summary>
        public int? UsuarioId { get; set; }
    
        /// <summary>
        /// Identificador del método de pago utilizado.
        /// </summary>
        public int? MetodoPagoId { get; set; }
    
        /// <summary>
        /// Identificador del estado actual del pedido.
        /// </summary>
        public int? EstadoPedidoId { get; set; }
    
        /// <summary>
        /// Identificador único de la transacción de pago.
        /// Máximo 255 caracteres.
        /// </summary>
        [StringLength(255)]
        public string? TransaccionId { get; set; }
    
        /// <summary>
        /// Fecha y hora en que se realizó el pedido.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Fecha { get; set; }
    
        /// <summary>
        /// Identificador del cupón de descuento aplicado al pedido.
        /// </summary>
        public int? CuponId { get; set; }
    
        /// <summary>
        /// Monto subtotal del pedido antes de impuestos y descuentos.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Subtotal { get; set; }
    
        /// <summary>
        /// Monto del impuesto aplicado al pedido.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Impuesto { get; set; }
    
        /// <summary>
        /// Monto del descuento aplicado al pedido.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Descuento { get; set; }
    
        /// <summary>
        /// Monto total del pedido incluyendo impuestos y descuentos.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Total { get; set; }
    
        /// <summary>
        /// Indica si el pedido está activo en el sistema.
        /// </summary>
        public bool? Activo { get; set; }
    
        /// <summary>
        /// Referencia al cupón de descuento aplicado al pedido.
        /// </summary>
        [ForeignKey("CuponId")]
        [InverseProperty("Pedidos")]
        public virtual Cupone? Cupon { get; set; }
    
        /// <summary>
        /// Colección de detalles que componen el pedido.
        /// </summary>
        [InverseProperty("Pedido")]
        public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    
        /// <summary>
        /// Referencia al estado actual del pedido.
        /// </summary>
        [ForeignKey("EstadoPedidoId")]
        [InverseProperty("Pedidos")]
        public virtual EstadosPedido? EstadoPedido { get; set; }
    
        /// <summary>
        /// Referencia al método de pago utilizado.
        /// </summary>
        [ForeignKey("MetodoPagoId")]
        [InverseProperty("Pedidos")]
        public virtual MetodosPago? MetodoPago { get; set; }
    
        /// <summary>
        /// Referencia al usuario que realizó el pedido.
        /// </summary>
        [ForeignKey("UsuarioId")]
        [InverseProperty("Pedidos")]
        public virtual Usuario? Usuario { get; set; }
    }