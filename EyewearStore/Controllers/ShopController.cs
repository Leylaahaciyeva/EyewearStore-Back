using EyewearStore.Contexts;
using EyewearStore.Models;
using EyewearStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace EyewearStore.Controllers;

public class ShopController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ShopController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Women()
    {
        var womenProducts = await _context.Products.Where(x => x.Gender).Include(x => x.ProductImages).ToListAsync();
        return View(womenProducts);
    }
    public async Task<IActionResult> Men()
    {
        var menProducts = await _context.Products.Where(x => !x.Gender).Include(x => x.ProductImages).ToListAsync();
        return View(menProducts);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var product = await _context.Products.Include(x => x.ProductImages).Include(x => x.Category).Include(x => x.Comments).ThenInclude(x => x.AppUser).FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();

        var relatedProducts = await _context.Products.Where(x => x.Gender == product.Gender || x.CategoryId == product.CategoryId).Where(x => x.Id != id).Take(3).Include(x => x.ProductImages).ToListAsync();

        ShopVM vm = new()
        {
            Product = product,
            RelatedProducts = relatedProducts,
        };

        ViewBag.Id = id;

        return View(vm);

    }
    [Authorize]
    public async Task<IActionResult> PostComment(int id, string Text, int Rating)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Detail", new { id });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var product = await _context.Products.Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();


        Comment comment = new()
        {
            AppUser = user,
            CreatedTime = DateTime.UtcNow,
            ProductId = id,
            Rating = Rating,
            Text = Text

        };

        decimal avarageRating = 0;

        foreach (var item in product.Comments)
        {
            avarageRating+= item.Rating;

        }

        avarageRating += Rating;


        avarageRating=(avarageRating)/(product.Comments.Count()+1);


        product.Rating = (int)Math.Ceiling(avarageRating);
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();


        return RedirectToAction("Detail", new { id });

    }
}
