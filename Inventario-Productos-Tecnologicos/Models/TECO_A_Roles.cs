using Microsoft.AspNetCore.Identity;

namespace Inventario_Productos_Tecnologicos.Models;

public class TECO_A_Roles : IdentityRole
{
    /// <summary>
    /// Indica si el rol está activo en el sistema.
    /// </summary>
    public bool TB_Activo { get; set; }
}