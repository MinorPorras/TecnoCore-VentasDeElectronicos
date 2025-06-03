using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Models;

public partial class Direccione
{
    [Key]
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    [StringLength(200)]
    public string Calle { get; set; } = null!;

    [StringLength(100)]
    public string Ciudad { get; set; } = null!;

    [StringLength(100)]
    public string? Provincia { get; set; }

    [StringLength(20)]
    public string CodigoPostal { get; set; } = null!;

    [StringLength(50)]
    public string? TipoDireccion { get; set; }

    public bool? Activo { get; set; }

    [ForeignKey("UsuarioId")]
    [InverseProperty("Direcciones")]
    public virtual Usuario? Usuario { get; set; }
}
