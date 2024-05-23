using EyewearStore.Areas.Admin.ViewModels;
using EyewearStore.Contexts;
using EyewearStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyewearStore.Areas.Admin.Controllers;
[Area("Admin")]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.ToListAsync();
        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var isExist = await _context.Categories.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower());

        if (isExist)
        {
            ModelState.AddModelError("Name", "This name is already exist");
            return View(vm);
        }

        Category category = new()
        {
            Name = vm.Name,
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
            return NotFound();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Update(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category is null)
            return NotFound();


        CategoryUpdateVM vm = new()
        {
            Name = category.Name,
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, CategoryUpdateVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var isExist = await _context.Categories.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower() && x.Id!=id);

        if (isExist)
        {
            ModelState.AddModelError("Name", "This name is already exist");
            return View(vm);
        }


        var existCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (existCategory is null)
            return NotFound();

        existCategory.Name = vm.Name;

        _context.Categories.Update(existCategory);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
