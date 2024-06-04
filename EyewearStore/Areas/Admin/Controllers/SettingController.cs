using EyewearStore.Contexts;
using EyewearStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyewearStore.Areas.Admin.Controllers;
[Area("Admin")]
public class SettingController : Controller
{
    private readonly AppDbContext _context;

    public SettingController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _context.Settings.ToListAsync();
        return View(settings);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Setting setting)
    {
        if (!ModelState.IsValid)
            return View(setting);

        var isExist = await _context.Settings.AnyAsync(x => x.Key == setting.Key);

        if (isExist)
        {
            ModelState.AddModelError("Key", "This key is already exist");
            return View(setting);

        }

        await _context.Settings.AddAsync(setting);
        await _context.SaveChangesAsync();


        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Id == id);

        if (setting is null)
            return NotFound();

        return View(setting);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, Setting setting)
    {
        if (!ModelState.IsValid)
            return View();

        var existSetting = await _context.Settings.FirstOrDefaultAsync(x => x.Id == id);

        if (existSetting is null)
            return NotFound();

        existSetting.Key = setting.Key;
        existSetting.Value = setting.Value;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");

    }

    public async Task<IActionResult> Delete(int id)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Id == id);

        if (setting is null)
            return NotFound();

        _context.Settings.Remove(setting);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
