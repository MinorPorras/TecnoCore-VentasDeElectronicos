using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

public class CarritoCompras
{
    [Key]
    [Column(Order = 0)]
    public int UsuarioId { get; set; }
    
    [Key]
    [Column(Order = 1)]
    public int ProductoId { get; set; }
    
    public int Cantidad { get; set; }
    
    [Column(TypeName = "decimal(10, 2)")]
    public decimal PrecioUnitario { get; set; }
    
    // Propiedades de navegación
    public virtual Usuarios Usuario { get; set; }
    public virtual Productos Producto { get; set; }
}
