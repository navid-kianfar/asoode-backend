using Asoode.Shared.Database.Tables;
using Asoode.Shared.Database.Tables.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Contexts;

internal class StorageDbContext : DbContext
{
    public StorageDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }
    
    public DbSet<Upload> Uploads { get; set; }
    public DbSet<UserPlanInfo> UserPlanInfo { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}