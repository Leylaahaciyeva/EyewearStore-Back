using System.ComponentModel.DataAnnotations;

namespace EyewearStore.ViewModels;

public class ContactVM
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Message { get; set; } = null!;
}