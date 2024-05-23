using Microsoft.EntityFrameworkCore;

namespace EyewearStore.Contexts;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
            
    }
}
