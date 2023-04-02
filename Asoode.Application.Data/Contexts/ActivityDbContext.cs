using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Application.Data.Contexts
{
    public class ActivityDbContext : DbContext
    {
        public ActivityDbContext(DbContextOptions<ActivityDbContext> options) : base(options)
        {
        }

        public DbSet<ActivityLog> Activities { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<WebPush> WebPushes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkPackage> WorkPackages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorkPackageLabel> WorkPackageLabels { get; set; }
        public DbSet<WorkPackageTask> WorkPackageTasks { get; set; }
        public DbSet<WorkPackageTaskMember> WorkPackageTaskMember { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<WorkPackageMember> WorkPackageMembers { get; set; }
        public DbSet<Channel> Channels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ContextOverrides.OnModelCreating(modelBuilder);
        }
    }
}