using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa un elemento de la lista de deseos de un usuario.
    /// </summary>
    public partial class ListaDeseos
    {
        /// <summary>
        /// Identificador único del elemento en la lista de deseos.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Identificador del usuario propietario de esta lista de deseos.
        /// </summary>
        public int? UsuarioId { get; set; }
    
        /// <summary>
        /// Identificador del producto agregado a la lista de deseos.
        /// </summary>
        public int? ProductoId { get; set; }
    
        /// <summary>
        /// Fecha y hora en que se agregó el producto a la lista de deseos.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? FechaAgregado { get; set; }
    
        /// <summary>
        /// Indica si el elemento de la lista de deseos está activo en el sistema.
        /// </summary>
        public bool? Activo { get; set; }
    
        /// <summary>
        /// Referencia al producto agregado a la lista de deseos.
        /// </summary>
        [ForeignKey("ProductoId")]
        [InverseProperty("ListaDeseos")]
        public virtual Productos? Producto { get; set; }
    
        /// <summary>
        /// Referencia al usuario propietario de esta lista de deseos.
        /// </summary>
        [ForeignKey("UsuarioId")]
        [InverseProperty("ListaDeseos")]
        public virtual Usuarios? Usuario { get; set; }
    }