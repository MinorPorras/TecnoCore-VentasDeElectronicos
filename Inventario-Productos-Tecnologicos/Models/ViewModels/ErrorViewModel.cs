namespace Inventario_Productos_Tecnologicos.Models;

/// <summary>
/// Modelo de vista para mostrar información de errores en la aplicación.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Identificador único de la solicitud que generó el error.
    /// Puede ser nulo si no está disponible.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Indica si se debe mostrar el identificador de solicitud.
    /// Retorna true si RequestId tiene un valor no nulo ni vacío.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}