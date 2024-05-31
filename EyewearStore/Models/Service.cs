using EyewearStore.Models.Common;

namespace EyewearStore.Models;

public class Service: BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Icon { get; set; } = null!;
}
