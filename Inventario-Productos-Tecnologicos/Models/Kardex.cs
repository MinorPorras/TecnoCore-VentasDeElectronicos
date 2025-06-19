using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa el registro de movimientos de inventario (kardex) de los productos.
    /// </summary>
    [Table("KARDEX")]
    public partial class Kardex
    {
        /// <summary>
        /// Identificador único del registro de kardex.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Identificador del producto al que pertenece este movimiento.
        /// </summary>
        public int? ProductoId { get; set; }
    
        /// <summary>
        /// Cantidad de unidades involucradas en el movimiento.
        /// </summary>
        public int? Cantidad { get; set; }
    
        /// <summary>
        /// Descripción detallada del movimiento realizado.
        /// Máximo 300 caracteres.
        /// </summary>
        [StringLength(300)]
        public string? Descripcion { get; set; }
    
        /// <summary>
        /// Fecha y hora en que se realizó el movimiento.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Fecha { get; set; }
    
        /// <summary>
        /// Cantidad en stock antes del movimiento.
        /// </summary>
        public int? StockAnterior { get; set; }
    
        /// <summary>
        /// Cantidad en stock después del movimiento.
        /// </summary>
        public int? StockActual { get; set; }
    
        /// <summary>
        /// Identificador del tipo de movimiento realizado.
        /// </summary>
        public int? TipoMovimientoId { get; set; }
    
        /// <summary>
        /// Indica si el registro del kardex está activo en el sistema.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Referencia al producto asociado a este movimiento.
        /// </summary>
        [ForeignKey("ProductoId")]
        [InverseProperty("Kardex")]
        public virtual Productos? Producto { get; set; }
    
        /// <summary>
        /// Referencia al tipo de movimiento realizado.
        /// </summary>
        [ForeignKey("TipoMovimientoId")]
        [InverseProperty("Kardex")]
        public virtual TipoMovimientoKardex? TipoMovimientoKardex { get; set; }
    }