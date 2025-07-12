namespace Inventario_Productos_Tecnologicos.Models.ViewModels;

public class Alert
{
    public string Type { get; set; }
    public string Message { get; set; }

    public Alert(string type = "info", string message = "")
    {
        Type = type;
        Message = message;
    }

    public static Alert NotFoundAlert(string name)
    {
        return new Alert("danger", $"No se encontró {name}");
    }

    public static Alert SuccessAlert()
    {
        return new Alert("success", $"Datos almacenados exitosamente");
    }

    public static Alert InfoAlert(string info)
    {
        return new Alert("info", info);
    }

    public static Alert ErrorAlert(string error)
    {
        return new Alert("danger", error);
    }
}