using EyewearStore.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace EyewearStore.Models;

public class Comment : BaseEntity
{
    public string Text { get; set; } = null!;
    [Range(0,5)]
    public int Rating { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }
    public AppUser AppUser { get; set; } = null!;
    public string AppUserId { get; set; }=null!;
}