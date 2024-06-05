using Microsoft.AspNetCore.Mvc;

namespace EyewearStore.Controllers;

public class BasketController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
