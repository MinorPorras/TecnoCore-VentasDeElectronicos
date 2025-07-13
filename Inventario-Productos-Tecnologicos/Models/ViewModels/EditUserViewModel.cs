using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem

namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class EditUserViewModel
{
    [Required] // El ID es crucial para saber qué usuario se está editando
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [Display(Name = "Usuario")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
    [Display(Name = "Correo")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El número de teléfono debe contener exactamente 8 dígitos.")]
    [Required(ErrorMessage = "El número de telefono es obligatorio.")]
    [Display(Name = "Número de Teléfono")]
    public string PhoneNumber { get; set; } = string.Empty;

    // --- Campos de Dirección ---
    [Required(ErrorMessage = "La dirección exacta es obligatoria.")]
    [StringLength(200)]
    [Display(Name = "Dirección Exacta")]
    public string DireccionExacta { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código postal es obligatorio.")]
    [StringLength(20)]
    [Display(Name = "Código Postal")]
    public string CodigoPostal { get; set; } = string.Empty;

    // --- Propiedades para los Dropdowns (Provincias y Cantones) ---
    [Required(ErrorMessage = "Debe seleccionar una provincia.")]
    [Display(Name = "Provincia")]
    public int SelectedProvinciaId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un cantón.")]
    [Display(Name = "Cantón")]
    public int? SelectedCantonId { get; set; }

    // Listas para poblar los Dropdowns en la vista (pueden ser nulas si se cargan en el controlador)
    public List<SelectListItem>? Provincias { get; set; } = [];
    public List<SelectListItem>? Cantones { get; set; } = [];

    [Required(ErrorMessage = "El rol es obligatorio")]
    [Display(Name = "Rol")]
    public string Rol { get; set; } = string.Empty; // Este es el ID del rol seleccionado
}