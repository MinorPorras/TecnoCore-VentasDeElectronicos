using System.ComponentModel.DataAnnotations;

namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class KardexViewModel
{
    public int? ProductoId { get; set; }
    public string? ProductoNombre { get; set; }

    [Required(ErrorMessage = "La fecha es requerida")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El tipo de movimiento es requerido")]
    public int TipoMovimientoId { get; set; }

    public int? StockAnterior { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    public int? StockActual { get; set; }

    [MaxLength(300, ErrorMessage = "La descripción no puede exceder los 300 caracteres")]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public List<TECO_A_Producto>? ProductosDisponibles { get; set; }
}