using EyewearStore.Models.Common;

namespace EyewearStore.Models;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public bool Gender { get; set; }
    public string Size { get; set; } = null!;
    public string LensInformations { get; set; } = null!;
    public int Rating { get; set; } = 5;
    public Category Category { get; set; } = null!;
    public int CategoryId { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; }=new List<ProductImage>();
    public ICollection<Comment> Comments { get; set; }=new List<Comment>();
 
}
