﻿using System.ComponentModel.DataAnnotations;

namespace EyewearStore.Areas.Admin.ViewModels;

public class ProductCreateVM
{

    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    [MinLength(15)]
    public string Description { get; set; } = null!;
    public bool Gender { get; set; }
    public string Size { get; set; } = null!;
    public string LensInformations { get; set; } = null!;
    public int CategoryId { get; set; }
    public IFormFile MainImage { get; set; } = null!;
    public IFormFile HoverImage { get; set; } = null!;
    public ICollection<IFormFile> AdditionalImages { get; set; } = new List<IFormFile>();
}



public class ProductUpdateVM
{

    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    [MinLength(15)]
    public string Description { get; set; } = null!;
    public bool Gender { get; set; }
    public string Size { get; set; } = null!;
    public string LensInformations { get; set; } = null!;
    public int CategoryId { get; set; }
    public string? MainImagePath { get; set; }
    public IFormFile? MainImage { get; set; } = null!;
    public string? HoverImagePath { get; set; }
    public IFormFile? HoverImage { get; set; } = null!;
    public List<string> AdditionalImagePaths { get; set; } = new();
    public List<int> AdditionalImageIds { get; set; } = new();
    public ICollection<IFormFile> AdditionalImages { get; set; } = new List<IFormFile>();
}