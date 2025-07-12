using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

public class TECO_M_Canton
{
    [Key] // La clave primaria se detecta automáticamente como autoincremental si es int
    public int TN_Id { get; set; } // Cambiado de int? a int

    [Required(ErrorMessage = "El nombre del cantón es obligatorio.")] // Agregado para validación
    [StringLength(100)] // Opcional, para limitar la longitud
    public string TC_Nombre { get; set; } = string.Empty; // Cambiado de string? a string y String.Empty

    // Clave Foránea a Provincia
    [Required(ErrorMessage = "La provincia es obligatoria para el cantón.")] // Agregado para validación
    public int TN_ProvinciaId { get; set; } // Cambiado de int? a int

    [ForeignKey("TN_ProvinciaId")]
    [InverseProperty("Canton")] // Indica que esta es la propiedad inversa a 'Cantones' en Provincia
    public virtual TECO_M_Provincia? Provincia { get; set; }

    // Propiedad de navegación para la relación con Direcciones
    // Un Cantón puede tener muchas Direcciones
    [InverseProperty("Canton")] // Indica que esta es la propiedad inversa a 'Canton' en Direcciones
    public ICollection<TECO_A_Direccion>? Direccion { get; set; } // Añadido para la relación 1 a Muchos
}