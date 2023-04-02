using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Application.Data.Contexts
{
    public class GeneralDbContext : DbContext
    {
        public GeneralDbContext(DbContextOptions<GeneralDbContext> options) : base(options)
        {
        }

        public DbSet<WorkPackageTaskAttachment> WorkPackageTaskAttachments { get; set; }
        public DbSet<Upload> Uploads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ContextOverrides.OnModelCreating(modelBuilder);
        }
    }
}