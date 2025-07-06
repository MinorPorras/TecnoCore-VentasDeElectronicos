using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

public class Provincia
{
    [Key] // La clave primaria se detecta automáticamente como autoincremental si es int
    public int Id { get; set; } // Cambiado de int? a int

    [Required(ErrorMessage = "El nombre de la provincia es obligatorio.")] // Agregado para validación
    [StringLength(100)] // Opcional, para limitar la longitud
    public string Nombre { get; set; } = string.Empty;

    // Propiedad de navegación para la relación con Cantones
    // Una Provincia puede tener muchos Cantones
    [InverseProperty("Provincia")] // Indica que esta es la propiedad inversa a 'Provincia' en Canton
    public ICollection<Canton>? Cantones { get; set; }
}