using EyewearStore.Contexts;
using EyewearStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyewearStore.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var trendProducts = await _context.Products.OrderByDescending(x => x.Price).Take(3).Include(x=>x.ProductImages).ToListAsync();
        var services = await _context.Services.ToListAsync();

        HomeVM vm = new()
        {
            Services = services,
            Products = trendProducts
        };

        return View(vm);
    }

}
