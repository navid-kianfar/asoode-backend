using Asoode.Data.Models;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Data.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
    }

    public DbSet<AdvancedPlayerComment> AdvancedPlayerComments { get; set; }
    public DbSet<AdvancedPlayerShape> AdvancedPlayerShapes { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<Wallet> Wallet { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<ContactReply> ContactReplies { get; set; }
    public DbSet<WorkPackageTaskTime> WorkPackageTaskTimes { get; set; }
    public DbSet<WorkPackageTaskVote> WorkPackageTaskVotes { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<PlanMember> PlanMembers { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserPlanInfo> UserPlanInfo { get; set; }
    public DbSet<ProjectSeason> ProjectSeasons { get; set; }
    public DbSet<WorkPackageRelatedTask> WorkPackageRelatedTasks { get; set; }
    public DbSet<WorkPackageView> WorkPackageViews { get; set; }

    public DbSet<WorkPackageCustomField> WorkPackageCustomFields { get; set; }
    public DbSet<WorkPackageCustomFieldItem> WorkPackageCustomFieldItems { get; set; }
    public DbSet<WorkPackageTaskCustomField> WorkPackageTaskCustomFields { get; set; }

    public DbSet<WorkPackageListMember> WorkPackageListMembers { get; set; }
    public DbSet<WorkPackageTaskMember> WorkPackageTaskMember { get; set; }
    public DbSet<WorkPackageTaskObjective> WorkPackageTaskObjectives { get; set; }
    public DbSet<WorkPackageTaskLabel> WorkPackageTaskLabels { get; set; }
    public DbSet<WorkPackageMember> WorkPackageMembers { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<WorkPackageObjective> WorkPackageObjectives { get; set; }
    public DbSet<WorkPackageTaskBlocker> WorkPackageTaskBlockers { get; set; }
    public DbSet<WorkPackageTaskComment> WorkPackageTaskComments { get; set; }
    public DbSet<WorkPackageTaskAttachment> WorkPackageTaskAttachments { get; set; }
    public DbSet<WorkPackageLabel> WorkPackageLabels { get; set; }
    public DbSet<ConversationSeen> ConversationSeens { get; set; }
    public DbSet<UploadAccess> UploadAccesses { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<WorkingTime> WorkingTimes { get; set; }
    public DbSet<UserShift> UserShifts { get; set; }
    public DbSet<TimeOff> TimeOffs { get; set; }
    public DbSet<TimeSpent> TimeSpents { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<SubProject> SubProjects { get; set; }
    public DbSet<WorkPackage> WorkPackages { get; set; }
    public DbSet<WorkPackageList> WorkPackageLists { get; set; }
    public DbSet<WorkPackageTask> WorkPackageTasks { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<ActivityLog> Activities { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<MarketerIncome> MarketerIncomes { get; set; }
    public DbSet<Marketer> Marketers { get; set; }
    public DbSet<Upload> Uploads { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserVerification> UserVerifications { get; set; }
    public DbSet<WebPush> WebPushes { get; set; }
    public DbSet<PendingInvitation> PendingInvitations { get; set; }
    public DbSet<WorkPackageTaskInteraction> WorkPackageTaskInteractions { get; set; }
    public DbSet<WorkPackageMemberSetting> WorkPackageMemberSettings { get; set; }

    public DbSet<SupportContact> SupportContacts { get; set; }
    public DbSet<SupportReply> SupportReplies { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogCategory> BlogCategories { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ContextOverrides.OnModelCreating(modelBuilder);
    }
}