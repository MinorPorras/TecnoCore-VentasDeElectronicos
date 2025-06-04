using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa una subcategoría de productos en el sistema de inventario.
    /// </summary>
    public partial class Subcategoria
    {
        /// <summary>
        /// Identificador único de la subcategoría.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Nombre de la subcategoría.
        /// Máximo 100 caracteres.
        /// </summary>
        [Column("NOMBRE")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;
    
        /// <summary>
        /// Identificador de la categoría principal a la que pertenece esta subcategoría.
        /// </summary>
        [Column("CategoriaID")]
        public int? CategoriaId { get; set; }
    
        /// <summary>
        /// Indica si la subcategoría está activa en el sistema.
        /// </summary>
        public bool? Activo { get; set; }
    
        /// <summary>
        /// Referencia a la categoría principal a la que pertenece esta subcategoría.
        /// </summary>
        [ForeignKey("CategoriaId")]
        [InverseProperty("Subcategoria")]
        public virtual Categoria? Categoria { get; set; }
    
        /// <summary>
        /// Colección de productos que pertenecen a esta subcategoría.
        /// </summary>
        [InverseProperty("Subcategoria")]
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }