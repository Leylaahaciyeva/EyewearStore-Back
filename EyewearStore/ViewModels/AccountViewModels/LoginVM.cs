using System.ComponentModel.DataAnnotations;

namespace EyewearStore.ViewModels;



public class LoginVM
{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}