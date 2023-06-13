using Asoode.Data.Models;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Data.Contexts;

public class CollaborationDbContext : DbContext
{
    public CollaborationDbContext(DbContextOptions<CollaborationDbContext> options) : base(options)
    {
    }

    public DbSet<UserShift> UserShifts { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<TimeOff> TimeOffs { get; set; }
    public DbSet<PlanMember> PlanMembers { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserPlanInfo> UserPlanInfo { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Upload> Uploads { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ActivityLog> Activities { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<WorkPackage> WorkPackages { get; set; }
    public DbSet<WorkPackageTaskMember> WorkPackageTaskMember { get; set; }
    public DbSet<WorkPackageMember> WorkPackageMembers { get; set; }
    public DbSet<PendingInvitation> PendingInvitations { get; set; }
    public DbSet<WorkingTime> WorkingTimes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}