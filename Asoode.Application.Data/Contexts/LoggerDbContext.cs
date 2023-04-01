using Asoode.Application.Data.Models;

namespace Asoode.Application.Data.Contexts
{
    public class LoggerDbContext : DbContext
    {
        public LoggerDbContext(DbContextOptions<LoggerDbContext> options) : base(options)
        {
        }

        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ContextOverrides.OnModelCreating(modelBuilder);
        }
    }
}