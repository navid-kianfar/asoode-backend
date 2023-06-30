using Asoode.Shared.Database.Tables;
using Asoode.Shared.Database.Tables.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Contexts;

internal class ReportsContext : DbContext
{
    public ReportsContext(DbContextOptions<ReportsContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<WorkPackage> WorkPackages { get; set; }
    public DbSet<WorkPackageMember> WorkPackageMembers { get; set; }
    public DbSet<WorkPackageTaskTime> WorkPackageTaskTimes { get; set; }
    public DbSet<WorkPackageList> WorkPackageLists { get; set; }
    public DbSet<WorkPackageTask> WorkPackageTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}