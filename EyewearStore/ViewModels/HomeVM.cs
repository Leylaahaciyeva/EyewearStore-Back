using EyewearStore.Models;

namespace EyewearStore.ViewModels;

public class HomeVM
{
    public List<Product> Products { get; set; } = new();
    public List<Service> Services { get; set; } = new();
    public Product BestProduct { get; set; }=new();
}
