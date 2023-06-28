using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Contexts;

internal class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
}