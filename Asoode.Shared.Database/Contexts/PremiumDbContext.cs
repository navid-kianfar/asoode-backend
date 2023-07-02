using Asoode.Shared.Database.Tables;
using Asoode.Shared.Database.Tables.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Contexts;

internal class PremiumDbContext : DbContext
{
    public PremiumDbContext(DbContextOptions<PremiumDbContext> options) : base(options)
    {
    }
    
    public DbSet<Wallet> Wallet { get; set; }
    public DbSet<PlanMember> PlanMembers { get; set; }
    
    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserPlanInfo> UserPlanInfo { get; set; }
    public DbSet<Marketer> Marketers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Transaction> Transaction { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}