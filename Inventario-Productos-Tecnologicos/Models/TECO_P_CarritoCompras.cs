using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

public class TECO_P_CarritoCompras
{
    [Key] [Column(Order = 0)] public string? TN_UsuarioId { get; set; }

    [Key] [Column(Order = 1)] public int TN_ProductoId { get; set; }

    public int TN_Cantidad { get; set; }

    [Column(TypeName = "decimal(10, 2)")] public decimal TN_PrecioUnitario { get; set; }

    // Propiedades de navegación
    [InverseProperty("CarritoCompras")] public virtual TECO_A_Usuario? Usuario { get; set; }
    [InverseProperty("CarritoCompras")] public virtual TECO_A_Producto? Producto { get; set; }
}