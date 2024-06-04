using EyewearStore.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyewearStore.Controllers;

public class ShopController : Controller
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Women()
    {
        var womenProducts=await _context.Products.Where(x=>x.Gender).Include(x=>x.ProductImages).ToListAsync();
        return View(womenProducts);
    }
    public async Task<IActionResult> Men()
    {
        var menProducts = await _context.Products.Where(x => !x.Gender).Include(x=>x.ProductImages).ToListAsync();
        return View(menProducts);
    }

 
}
