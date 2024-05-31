using Microsoft.AspNetCore.Mvc;

namespace EyewearStore.Controllers;

public class ContactController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
