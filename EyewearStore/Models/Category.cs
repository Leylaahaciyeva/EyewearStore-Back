using EyewearStore.Models.Common;

namespace EyewearStore.Models;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
