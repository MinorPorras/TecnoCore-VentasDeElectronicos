using Microsoft.AspNetCore.Mvc;

namespace Inventario_Productos_Tecnologicos.webcomponent;

public class DeleteDialogViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string actionName, string controllerName)
    {
        Console.WriteLine(actionName);
        Console.WriteLine(controllerName);
        ViewBag.ActionName = actionName;
        ViewBag.ControllerName = controllerName;
        return View();
    }
}