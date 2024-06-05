using EyewearStore.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace EyewearStore.Models;

public class BasketItem : BaseEntity
{
    [Range(0,int.MaxValue)]
    public int Count { get; set; }
    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }
    public AppUser AppUser { get; set; }=null!;
    public string AppUserId { get; set; } = null!;
}