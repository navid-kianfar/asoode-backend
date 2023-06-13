using Asoode.Data.Models;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Data.Contexts;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<WebPush> WebPushes { get; set; }
    public DbSet<WorkingTime> WorkingTimes { get; set; }
    public DbSet<WorkPackageTaskTime> WorkPackageTaskTimes { get; set; }

    public DbSet<PlanMember> PlanMembers { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserPlanInfo> UserPlanInfo { get; set; }
    public DbSet<Marketer> Marketers { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Wallet> Wallet { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserVerification> UserVerifications { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Transaction> Transaction { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}