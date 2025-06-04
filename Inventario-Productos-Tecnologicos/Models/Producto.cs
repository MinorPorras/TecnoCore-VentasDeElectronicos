using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa un producto en el inventario del sistema.
    /// </summary>
    public partial class Producto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Nombre del producto.
        /// Máximo 100 caracteres.
        /// </summary>
        [StringLength(100)]
        public string Nombre { get; set; } = null!;
    
        /// <summary>
        /// Descripción detallada del producto.
        /// Máximo 300 caracteres.
        /// </summary>
        [StringLength(300)]
        public string? Descripcion { get; set; }
    
        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Precio { get; set; }
    
        /// <summary>
        /// Cantidad disponible en inventario.
        /// </summary>
        public int? Stock { get; set; }
    
        /// <summary>
        /// Ruta de la imagen del producto.
        /// Máximo 255 caracteres.
        /// </summary>
        [StringLength(255)]
        public string? Imagen { get; set; }
    
        /// <summary>
        /// Identificador de la subcategoría a la que pertenece el producto.
        /// </summary>
        public int? SubcategoriaId { get; set; }
    
        /// <summary>
        /// Indica si el producto está activo en el sistema.
        /// </summary>
        public bool? Activo { get; set; }
    
        /// <summary>
        /// Colección de detalles de pedidos que incluyen este producto.
        /// </summary>
        [InverseProperty("Producto")]
        public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    
        /// <summary>
        /// Colección de registros de kardex asociados a este producto.
        /// </summary>
        [InverseProperty("Producto")]
        public virtual ICollection<Kardex> Kardices { get; set; } = new List<Kardex>();
    
        /// <summary>
        /// Colección de listas de deseos que incluyen este producto.
        /// </summary>
        [InverseProperty("Producto")]
        public virtual ICollection<ListaDeseo> ListaDeseos { get; set; } = new List<ListaDeseo>();
    
        /// <summary>
        /// Colección de atributos específicos asociados a este producto.
        /// </summary>
        [InverseProperty("Producto")]
        public virtual ICollection<ProductoAtributo> ProductoAtributos { get; set; } = new List<ProductoAtributo>();
    
        /// <summary>
        /// Referencia a la subcategoría a la que pertenece el producto.
        /// </summary>
        [ForeignKey("SubcategoriaId")]
        [InverseProperty("Productos")]
        public virtual Subcategoria? Subcategoria { get; set; }
    }