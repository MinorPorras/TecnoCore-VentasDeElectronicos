using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Identity;
using Inventario_Productos_Tecnologicos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario_Productos_Tecnologicos.Models.ViewModels;

namespace Inventario_Productos_Tecnologicos.Viewcomponent;

public class ClientesMasComprasViewComponent : ViewComponent
{
    private readonly TecnoCoreDbContext _context;
    private readonly ILogger<ClientesMasComprasViewComponent> _logger;
    private readonly UserManager<TECO_A_Usuario> _userManager;
    private readonly RoleManager<TECO_A_Roles> _roleManager;
    
    public ClientesMasComprasViewComponent(TecnoCoreDbContext context, ILogger<ClientesMasComprasViewComponent> logger, UserManager<TECO_A_Usuario> userManager, RoleManager<TECO_A_Roles> roleManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var roles = await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == "Cliente");
        if (roles == null)
        {
            _logger.LogWarning("No se encontró el rol 'Cliente'.");
            return View(new List<ClienteMasComprasViewModel>());
        }

        var topClientesConTotal = await _userManager.Users
            .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roles.Id))
            .Select(u => new ClienteMasComprasViewModel() // Usar el ViewModel aquí
            {
                Usuario = u,
                TotalCompras = _context.TECO_P_Pedido
                    .Where(p => p.TN_UsuarioId == u.Id)
                    .Sum(p => p.TN_Total)
            })
            .OrderByDescending(x => x.TotalCompras)
            .Take(3)
            .ToListAsync();
        
        return View(topClientesConTotal);
    }
}