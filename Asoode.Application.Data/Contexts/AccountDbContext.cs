using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Application.Data.Contexts
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        public DbSet<WebPush> WebPushes { get; set; }
        public DbSet<WorkingTime> WorkingTimes { get; set; }
        public DbSet<WorkPackageTaskTime> WorkPackageTaskTimes { get; set; }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ContextOverrides.OnModelCreating(modelBuilder);
        }
    }
}