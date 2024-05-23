using Microsoft.AspNetCore.Mvc;

namespace EyewearStore.Areas.Admin.Controllers;
[Area("Admin")]
public class DashboardController : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }
}
