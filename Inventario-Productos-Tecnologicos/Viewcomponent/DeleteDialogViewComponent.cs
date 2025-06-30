using Microsoft.AspNetCore.Mvc;

namespace Inventario_Productos_Tecnologicos.webcomponent;

public class DeleteDialogViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string actionName, string controllerName)
    {
        ViewBag.ActionName = actionName;
        ViewBag.ControllerName = controllerName;
        return View();
    }
}