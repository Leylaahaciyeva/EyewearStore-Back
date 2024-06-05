using EyewearStore.Models;

namespace EyewearStore.ViewModels;

public class ShopVM
{
    public Product Product { get; set; }=new();
    public List<Product> RelatedProducts { get; set; } = new();
}
