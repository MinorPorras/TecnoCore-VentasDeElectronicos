using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

public class Canton
{
    [Key] // La clave primaria se detecta automáticamente como autoincremental si es int
    public int Id { get; set; } // Cambiado de int? a int

    [Required(ErrorMessage = "El nombre del cantón es obligatorio.")] // Agregado para validación
    [StringLength(100)] // Opcional, para limitar la longitud
    public string Nombre { get; set; } = string.Empty; // Cambiado de string? a string y String.Empty

    // Clave Foránea a Provincia
    [Required(ErrorMessage = "La provincia es obligatoria para el cantón.")] // Agregado para validación
    public int ProvinciaId { get; set; } // Cambiado de int? a int

    [ForeignKey("ProvinciaId")]
    [InverseProperty("Cantones")] // Indica que esta es la propiedad inversa a 'Cantones' en Provincia
    public virtual Provincia? Provincia { get; set; }

    // Propiedad de navegación para la relación con Direcciones
    // Un Cantón puede tener muchas Direcciones
    [InverseProperty("Canton")] // Indica que esta es la propiedad inversa a 'Canton' en Direcciones
    public ICollection<Direcciones>? Direcciones { get; set; } // Añadido para la relación 1 a Muchos
}