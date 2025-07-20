namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class ApplyDiscountRequestViewModel
{
    public string codigoCupon { get; set; }
    public decimal totalCarrito { get; set; }
}