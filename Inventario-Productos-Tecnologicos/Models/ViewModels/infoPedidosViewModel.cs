namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class InfoPedidosViewModel
{
    public int Pendiente { get; set; }
    
    public int Confirmado { get; set; }

    public int EnProceso { get; set; }

    public int Enviado { get; set; }

    public int Entregado { get; set; }

    public int Cancelado { get; set; }

}