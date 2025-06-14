using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    
    namespace Inventario_Productos_Tecnologicos.Models;
    
    /// <summary>
    /// Representa una dirección de envío asociada a un usuario.
    /// </summary>
    public partial class Direcciones
    {
        /// <summary>
        /// Identificador único de la dirección.
        /// </summary>
        [Key]
        public int Id { get; set; }
    
        /// <summary>
        /// Identificador del usuario al que pertenece esta dirección.
        /// </summary>
        public int? UsuarioId { get; set; }
    
        /// <summary>
        /// Nombre de la calle y número.
        /// Máximo 200 caracteres.
        /// </summary>
        [StringLength(200)]
        public string Calle { get; set; } = null!;
    
        /// <summary>
        /// Ciudad donde se encuentra la dirección.
        /// Máximo 100 caracteres.
        /// </summary>
        [StringLength(100)]
        public string Ciudad { get; set; } = null!;
    
        /// <summary>
        /// Provincia o estado donde se encuentra la dirección.
        /// Máximo 100 caracteres.
        /// </summary>
        [StringLength(100)]
        public string? Provincia { get; set; }
    
        /// <summary>
        /// Código postal de la dirección.
        /// Máximo 20 caracteres.
        /// </summary>
        [StringLength(20)]
        public string CodigoPostal { get; set; } = null!;
    
        /// <summary>
        /// Tipo de dirección (ej: envío, facturación).
        /// Máximo 50 caracteres.
        /// </summary>
        [StringLength(50)]
        public string? TipoDireccion { get; set; }
    
        /// <summary>
        /// Indica si la dirección está activa en el sistema.
        /// </summary>
        public bool? Activo { get; set; }
    
        /// <summary>
        /// Referencia al usuario propietario de esta dirección.
        /// </summary>
        [ForeignKey("UsuarioId")]
        [InverseProperty("Direccion")]
        public virtual Usuarios? Usuario { get; set; }
    }