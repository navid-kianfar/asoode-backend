using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Junctions;

namespace Asoode.Application.Data.Contexts
{
    public class GeneralDbContext : DbContext
    {
        public GeneralDbContext(DbContextOptions<GeneralDbContext> options) : base(options)
        {
        }

        public DbSet<MarketerIncome> MarketerIncomes { get; set; }
        public DbSet<Marketer> Marketers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<WorkPackageTaskAttachment> WorkPackageTaskAttachments { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserPlanInfo> UserPlanInfo { get; set; }
        public DbSet<SupportContact> SupportContacts { get; set; }
        public DbSet<SupportReply> SupportReplies { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactReply> ContactReplies { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ContextOverrides.OnModelCreating(modelBuilder);
        }
    }
}