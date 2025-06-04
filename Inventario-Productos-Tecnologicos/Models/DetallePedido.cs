using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa el detalle de un pedido, incluyendo los productos y sus cantidades.
    /// </summary>
    public partial class DetallePedido
    {
        /// <summary>
        /// Identificador único del detalle de pedido.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Identificador del pedido al que pertenece este detalle.
        /// </summary>
        public int? PedidoId { get; set; }
    
        /// <summary>
        /// Identificador del producto incluido en este detalle.
        /// </summary>
        public int? ProductoId { get; set; }
    
        /// <summary>
        /// Cantidad del producto solicitada en el pedido.
        /// </summary>
        public int? Cantidad { get; set; }
    
        /// <summary>
        /// Precio unitario del producto al momento de realizar el pedido.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PrecioUnitario { get; set; }
    
        /// <summary>
        /// Indica si el detalle del pedido está activo.
        /// </summary>
        public bool? Activo { get; set; }
    
        /// <summary>
        /// Referencia al pedido al que pertenece este detalle.
        /// </summary>
        [ForeignKey("PedidoId")]
        [InverseProperty("DetallePedidos")]
        public virtual Pedido? Pedido { get; set; }
    
        /// <summary>
        /// Referencia al producto incluido en este detalle.
        /// </summary>
        [ForeignKey("ProductoId")]
        [InverseProperty("DetallePedidos")]
        public virtual Producto? Producto { get; set; }
    }