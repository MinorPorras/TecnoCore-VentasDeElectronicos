using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario_Productos_Tecnologicos.Models;

public class TECO_P_CarritoCompras
{
    //Cada registro en el carrito de compras está asociado a un usuario y un producto específico.
    // Osea cada fila es un producto específico que un usuario ha agregado a su carrito de compras.
    //Por lo que para obtener la información de carrito de compras del usuario se deben de seleciconar varias filas que esten vinculadas al usuario.
    [Key] [Column(Order = 0)] public string? TN_UsuarioId { get; set; }

    [Key] [Column(Order = 1)] public int TN_ProductoId { get; set; }

    public int TN_Cantidad { get; set; }

    [Column(TypeName = "decimal(10, 2)")] public decimal TN_PrecioUnitario { get; set; }

    // Propiedades de navegación
    [InverseProperty("CarritoCompras")] public virtual TECO_A_Usuario? Usuario { get; set; }
    
    [ForeignKey("TN_ProductoId")] // Especifica la clave foránea
    public virtual TECO_A_Producto? Producto { get; set; }
}