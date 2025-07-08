using Microsoft.AspNetCore.Identity;

namespace Inventario_Productos_Tecnologicos.Models;

public class Roles : IdentityRole
{
    /// <summary>
    /// Indica si el rol está activo en el sistema.
    /// </summary>
    public bool Activo { get; set; }
}