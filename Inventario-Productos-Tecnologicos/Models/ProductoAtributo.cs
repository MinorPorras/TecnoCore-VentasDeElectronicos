using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa la relación entre un producto y sus atributos específicos.
    /// </summary>
    public partial class ProductoAtributo
    {
        /// <summary>
        /// Identificador único de la relación producto-atributo.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Identificador del producto al que pertenece este atributo.
        /// </summary>
        public int? ProductoId { get; set; }
    
        /// <summary>
        /// Identificador del atributo asignado al producto.
        /// </summary>
        public int? AtributoId { get; set; }
    
        /// <summary>
        /// Valor específico del atributo para este producto.
        /// Máximo 255 caracteres.
        /// </summary>
        [StringLength(255)]
        public string Valor { get; set; } = null!;
    
        /// <summary>
        /// Indica si la relación producto-atributo está activa en el sistema.
        /// </summary>
        public bool? Activo { get; set; }
    
        /// <summary>
        /// Referencia al atributo base asignado al producto.
        /// </summary>
        [ForeignKey("AtributoId")]
        [InverseProperty("ProductoAtributos")]
        public virtual Atributo? Atributo { get; set; }
    
        /// <summary>
        /// Referencia al producto al que pertenece este atributo.
        /// </summary>
        [ForeignKey("ProductoId")]
        [InverseProperty("ProductoAtributos")]
        public virtual Producto? Producto { get; set; }
    }