using EyewearStore.Areas.Admin.ViewModels;
using EyewearStore.Contexts;
using EyewearStore.Extensions;
using EyewearStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyewearStore.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProductController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(x => x.ProductImages).Include(x => x.Category).ToListAsync();
        return View(products);
    }




    public async Task<IActionResult> Create()
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;

        if (!ModelState.IsValid)
            return View(vm);


        var isExistCategory=await _context.Categories.AnyAsync(x=>x.Id==vm.CategoryId);
        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "This category is not found");
            return View(vm);
        }

        if (!vm.MainImage.ValidateType("image"))
        {
            ModelState.AddModelError("MainImage", "Invalid image type");
            return View(vm);
        }
        if (!vm.MainImage.ValidateSize(2))
        {
            ModelState.AddModelError("MainImage", "Image max size-2 mb");
            return View(vm);
        }


        if (!vm.HoverImage.ValidateType("image"))
        {
            ModelState.AddModelError("HoverImage", "Invalid image type");
            return View(vm);
        }
        if (!vm.HoverImage.ValidateSize(2))
        {
            ModelState.AddModelError("HoverImage", "Image max size-2 mb");
            return View(vm);
        }


        foreach (var image in vm.AdditionalImages)
        {
            if (!image.ValidateType("image"))
            {
                ModelState.AddModelError("AdditionalImages", "Invalid image type");
                return View(vm);
            }
            if (!image.ValidateSize(2))
            {
                ModelState.AddModelError("AdditionalImages", "Image max size-2 mb");
                return View(vm);
            }
        }


        Product product = new()
        {
            Name = vm.Name,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
            Price = vm.Price,
            Gender = vm.Gender,
            LensInformations = vm.LensInformations,
            Size = vm.Size,

        };


        var mainImagePath = await vm.MainImage.FileCreateAsync(_environment.WebRootPath, "assets", "image");
        ProductImage mainImage = new() { IsMain = true, Path = mainImagePath, Product = product };
        product.ProductImages.Add(mainImage);


        var hoverImagePath = await vm.HoverImage.FileCreateAsync(_environment.WebRootPath,"assets", "image");
        ProductImage hoverImage = new() { IsHover = true, Path = hoverImagePath, Product = product };
        product.ProductImages.Add(hoverImage);


        foreach (var image in vm.AdditionalImages)
        {
            var imagePath = await image.FileCreateAsync(_environment.WebRootPath, "assets","image");
            ProductImage pImage = new() { Path = imagePath, Product = product };
            product.ProductImages.Add(pImage);
        }


        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();


        return RedirectToAction("Index");

    }


    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);


        if (product is null)
            return NotFound();


        foreach (var image in product.ProductImages)
        {
            image.Path.FileDelete(_environment.WebRootPath,"assets", "image");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


}
