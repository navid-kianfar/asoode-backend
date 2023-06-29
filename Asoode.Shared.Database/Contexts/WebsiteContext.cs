using Asoode.Shared.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Contexts;

internal class WebsiteContext : DbContext
{
    public WebsiteContext(DbContextOptions<WebsiteContext> options) : base(options)
    {
    }
    
    public DbSet<SupportContact> SupportContacts { get; set; }
    public DbSet<SupportReply> SupportReplies { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<ContactReply> ContactReplies { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogCategory> BlogCategories { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Plan> Plans { get; set; }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}