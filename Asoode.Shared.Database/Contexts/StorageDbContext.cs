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
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<WorkPackageMember> WorkPackageMembers { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<WorkPackage> WorkPackages { get; set; }
    public DbSet<WorkPackageTaskAttachment> WorkPackageTaskAttachments { get; set; }
    public DbSet<WorkPackageTask> WorkPackageTasks { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}