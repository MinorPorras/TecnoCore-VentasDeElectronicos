using System.Runtime.InteropServices.JavaScript;

namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class ReporteVentasViewModel
{ 
    public DateTime fechaInicio;
    public DateTime fechaFin;
    public decimal total;
    public decimal descuento;
    public string productoMejorVendido;
    public string marcaMejorVendida;
    public string subcategoriaMejorVendida;
    public List<TECO_P_Pedido> ListPedidos;
}