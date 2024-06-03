using Microsoft.AspNetCore.Identity;

namespace EyewearStore.Models;

public class AppUser : IdentityUser
{
    public string Fullname { get; set; } = null!;
}
