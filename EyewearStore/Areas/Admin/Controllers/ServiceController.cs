using EyewearStore.Areas.Admin.ViewModels;
using EyewearStore.Contexts;
using EyewearStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyewearStore.Areas.Admin.Controllers;

[Area("Admin")]

public class ServiceController : Controller
{
    private readonly AppDbContext _context;

    public ServiceController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var services = await _context.Services.ToListAsync();

        return View(services);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(ServiceCreateVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        Service service = new()
        {
            Title = vm.Title,
            Description = vm.Description,
            Icon = vm.Icon
        };
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

        if (service is null)
            return NotFound();

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();


        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Update(int id)
    {
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

        if (service is null)
            return NotFound();

        ServiceUpdateVM vm = new()
        {
            Title = service.Title,
            Description = service.Description,
            Icon = service.Icon
        };

        return View(vm);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int id, ServiceUpdateVM vm)
    {
        if (!ModelState.IsValid)
            return View();

        var existService = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

        if (existService is null)
            return NotFound();


        existService.Title = vm.Title;
        existService.Description = vm.Description;
        existService.Icon = vm.Icon;

        _context.Services.Update(existService);
        await _context.SaveChangesAsync();


        return RedirectToAction("Index");
    }
}
