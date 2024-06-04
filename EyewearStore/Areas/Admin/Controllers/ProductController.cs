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



    public async Task<IActionResult> Update(int id)
    {
        var product = await _context.Products.Include(x=>x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();


        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;


        ProductUpdateVM vm = new()
        {
            AdditionalImageIds=product.ProductImages.Where(x=>!x.IsMain && !x.IsHover).Select(x=>x.Id).ToList(),
            AdditionalImagePaths= product.ProductImages.Where(x => !x.IsMain && !x.IsHover).Select(x => x.Path).ToList(),
            CategoryId=product.CategoryId,
            Description=product.Description,
            Gender=product.Gender,
            HoverImagePath=product.ProductImages.FirstOrDefault(x=>x.IsHover)?.Path,
            LensInformations=product.LensInformations,
            MainImagePath=product.ProductImages.FirstOrDefault( x=>x.IsMain)?.Path,
            Name=product.Name,
            Price=product.Price,
            Size = product.Size
        };

        return View(vm);

    }

    [HttpPost]
    public async Task<IActionResult> Update(int id,ProductUpdateVM vm)
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;

        if (!ModelState.IsValid)
            return View(vm);


        var existProduct = await _context.Products.Include(x=>x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);


        if (existProduct is null)
            return NotFound();


        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);
        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "This category is not found");
            return View(vm);
        }


        #region ImageValidates



        if (vm.MainImage is not null && !vm.MainImage.ValidateImage(2))
        {
            ModelState.AddModelError("MainImage", "Resim doğru formatda ve boyutu 2 mb dan az olmalıdır");
            return View(vm);
        }



        if (vm.HoverImage is not null && !vm.HoverImage.ValidateImage(10))
        {
            ModelState.AddModelError("HoverImage", "Resim doğru formatda ve boyutu 10 mb dan az olmalıdır");
            return View(vm);
        }


        foreach (IFormFile image in vm.AdditionalImages)
        {
            if (!image.ValidateImage(2))
            {
                ModelState.AddModelError("Images", "Resim doğru formatda ve boyutu 2 mb dan az olmalıdır");
                return View(vm);
            }
        }


        #endregion



        var ExistedImages = existProduct.ProductImages.Where(x=>!x.IsMain && !x.IsHover).Select(x => x.Id).ToList();
        if (vm.AdditionalImageIds is not null)
        {
            ExistedImages = ExistedImages.Except(vm.AdditionalImageIds).ToList();

        }
        if (ExistedImages.Count > 0)
        {
            foreach (var imageId in ExistedImages)
            {
                var deletedImage = existProduct.ProductImages.FirstOrDefault(x => x.Id == imageId);
                if (deletedImage is not null)
                {

                    existProduct.ProductImages.Remove(deletedImage);
                    deletedImage.Path.FileDelete(_environment.WebRootPath, "assets", "image");

                }

            }
        }

        //Created new Images
        if (vm.AdditionalImages is not null)
        {
            foreach (var item in vm.AdditionalImages)
            {
                ProductImage productImage = new() { Path =await item.FileCreateAsync(_environment.WebRootPath, "assets", "image"), ProductId = id };
                existProduct.ProductImages.Add(productImage);

            }
        }
        if (vm.MainImage is not null)
            existProduct.ProductImages.FirstOrDefault(x=>x.IsMain).Path = await vm.MainImage.FileCreateAsync(_environment.WebRootPath, "assets", "image");
        if (vm.HoverImage is not null)
            existProduct.ProductImages.FirstOrDefault(x => x.IsHover).Path = await vm.HoverImage.FileCreateAsync(_environment.WebRootPath, "assets", "image");


        existProduct.Name = vm.Name;
        existProduct.Gender = vm.Gender;
        existProduct.Price = vm.Price;
        existProduct.CategoryId= vm.CategoryId;
        existProduct.Size = vm.Size;
        existProduct.Description = vm.Description;
        existProduct.LensInformations = vm.LensInformations;


        _context.Products.Update(existProduct);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }
}
