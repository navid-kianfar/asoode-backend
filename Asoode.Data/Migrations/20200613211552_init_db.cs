using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Asoode.Data.Migrations
{
    public partial class init_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "blogcategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    BlogId = table.Column<Guid>(nullable: false),
                    Culture = table.Column<string>(maxLength: 2, nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: false),
                    NormalizedTitle = table.Column<string>(maxLength: 1000, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blogcategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "blogposts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    BlogId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: true),
                    Culture = table.Column<string>(maxLength: 2, nullable: false),
                    Keywords = table.Column<string>(maxLength: 1000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Title = table.Column<string>(maxLength: 1000, nullable: false),
                    NormalizedTitle = table.Column<string>(maxLength: 1000, nullable: false),
                    Summary = table.Column<string>(maxLength: 2000, nullable: false),
                    Text = table.Column<string>(maxLength: 10000, nullable: false),
                    ThumbImage = table.Column<string>(maxLength: 500, nullable: true),
                    MediumImage = table.Column<string>(maxLength: 500, nullable: true),
                    LargeImage = table.Column<string>(maxLength: 500, nullable: true),
                    Key = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blogposts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "blogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Culture = table.Column<string>(maxLength: 2, nullable: false),
                    Keywords = table.Column<string>(maxLength: 1000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Title = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ArchivedAt = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 1000, nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    RootId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contactreplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ContactId = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactreplies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    Seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ChannelId = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(maxLength: 2000, nullable: true),
                    Path = table.Column<string>(maxLength: 500, nullable: true),
                    ReplyId = table.Column<Guid>(nullable: true),
                    UploadId = table.Column<Guid>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "conversationseens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ConversationId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversationseens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 500, nullable: true),
                    Code = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 1500, nullable: true),
                    StartAt = table.Column<DateTime>(nullable: true),
                    EndAt = table.Column<DateTime>(nullable: true),
                    ForUser = table.Column<Guid>(nullable: true),
                    MaxUsage = table.Column<int>(nullable: false),
                    Percent = table.Column<int>(nullable: false),
                    MaxUnit = table.Column<double>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    PlanId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "errorlogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    ErrorBody = table.Column<string>(maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_errorlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "groupmembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: false),
                    Access = table.Column<byte>(nullable: false),
                    RootId = table.Column<Guid>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groupmembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ArchivedAt = table.Column<DateTime>(nullable: true),
                    RegisteredAt = table.Column<DateTime>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 2000, nullable: true),
                    BrandTitle = table.Column<string>(maxLength: 2000, nullable: true),
                    SupervisorName = table.Column<string>(maxLength: 250, nullable: true),
                    SupervisorNumber = table.Column<string>(maxLength: 50, nullable: true),
                    ResponsibleName = table.Column<string>(maxLength: 250, nullable: true),
                    ResponsibleNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Website = table.Column<string>(maxLength: 500, nullable: true),
                    PostalCode = table.Column<string>(maxLength: 100, nullable: true),
                    Address = table.Column<string>(maxLength: 2000, nullable: true),
                    Tel = table.Column<string>(maxLength: 50, nullable: true),
                    Fax = table.Column<string>(maxLength: 50, nullable: true),
                    GeoLocation = table.Column<string>(maxLength: 100, nullable: true),
                    NationalId = table.Column<string>(maxLength: 100, nullable: true),
                    RegistrationId = table.Column<string>(maxLength: 100, nullable: true),
                    Offices = table.Column<int>(nullable: true),
                    Employees = table.Column<int>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    PlanInfoId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    ExpireAt = table.Column<DateTime>(nullable: true),
                    Avatar = table.Column<string>(maxLength: 500, nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    RootId = table.Column<Guid>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Premium = table.Column<bool>(nullable: false),
                    Complex = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "marketerincomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    BillAmount = table.Column<double>(nullable: false),
                    ClearedAt = table.Column<DateTime>(nullable: false),
                    EarnAmount = table.Column<double>(nullable: false),
                    Fixed = table.Column<int>(nullable: false),
                    MarketerId = table.Column<Guid>(nullable: false),
                    PaymentId = table.Column<Guid>(nullable: false),
                    Percent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketerincomes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "marketers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    Fixed = table.Column<int>(nullable: true),
                    Percent = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Avatar = table.Column<string>(maxLength: 250, nullable: true),
                    Data = table.Column<string>(maxLength: 2000, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Url = table.Column<string>(maxLength: 2000, nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    DiscountId = table.Column<Guid>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    PlanId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    OneTime = table.Column<bool>(nullable: false),
                    Days = table.Column<int>(nullable: false),
                    AttachmentSize = table.Column<int>(nullable: false),
                    CanExtend = table.Column<bool>(nullable: false),
                    PlanCost = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    AppliedDiscount = table.Column<double>(nullable: true),
                    ValueAdded = table.Column<double>(nullable: false),
                    PaymentAmount = table.Column<double>(nullable: false),
                    DiskSpace = table.Column<double>(nullable: false),
                    Users = table.Column<int>(nullable: false),
                    WorkPackage = table.Column<int>(nullable: false),
                    Project = table.Column<int>(nullable: false),
                    SimpleGroup = table.Column<int>(nullable: false),
                    ComplexGroup = table.Column<int>(nullable: false),
                    AdditionalUser = table.Column<int>(nullable: false),
                    AdditionalSpace = table.Column<long>(nullable: false),
                    AdditionalProject = table.Column<int>(nullable: false),
                    AdditionalSimpleGroup = table.Column<int>(nullable: false),
                    AdditionalComplexGroup = table.Column<int>(nullable: false),
                    AdditionalWorkPackage = table.Column<int>(nullable: false),
                    AdditionalWorkPackageCost = table.Column<int>(nullable: false),
                    AdditionalUserCost = table.Column<int>(nullable: false),
                    AdditionalSpaceCost = table.Column<int>(nullable: false),
                    AdditionalProjectCost = table.Column<int>(nullable: false),
                    AdditionalSimpleGroupCost = table.Column<int>(nullable: false),
                    AdditionalComplexGroupCost = table.Column<int>(nullable: false),
                    FeatureCustomField = table.Column<bool>(nullable: false),
                    FeatureTimeSpent = table.Column<bool>(nullable: false),
                    FeatureTimeValue = table.Column<bool>(nullable: false),
                    FeatureTimeOff = table.Column<bool>(nullable: false),
                    FeatureShift = table.Column<bool>(nullable: false),
                    FeatureReports = table.Column<bool>(nullable: false),
                    FeaturePayments = table.Column<bool>(nullable: false),
                    FeatureChat = table.Column<bool>(nullable: false),
                    FeatureFiles = table.Column<bool>(nullable: false),
                    FeatureWbs = table.Column<bool>(nullable: false),
                    FeatureRoadMap = table.Column<bool>(nullable: false),
                    FeatureTree = table.Column<bool>(nullable: false),
                    FeatureObjectives = table.Column<bool>(nullable: false),
                    FeatureSeasons = table.Column<bool>(nullable: false),
                    FeatureVote = table.Column<bool>(nullable: false),
                    FeatureSubTask = table.Column<bool>(nullable: false),
                    FeatureKartabl = table.Column<bool>(nullable: false),
                    FeatureCalendar = table.Column<bool>(nullable: false),
                    FeatureBlocking = table.Column<bool>(nullable: false),
                    FeatureRelated = table.Column<bool>(nullable: false),
                    FeatureComplexGroup = table.Column<bool>(nullable: false),
                    FeatureGroupTimeSpent = table.Column<bool>(nullable: false),
                    UseWallet = table.Column<bool>(nullable: false),
                    ExpireAt = table.Column<DateTime>(nullable: false),
                    Automated = table.Column<bool>(nullable: false),
                    OrderType = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pendinginvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Identifier = table.Column<string>(nullable: true),
                    RecordId = table.Column<Guid>(nullable: false),
                    Access = table.Column<byte>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Canceled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pendinginvitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "planmembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PlanId = table.Column<Guid>(nullable: false),
                    Identifier = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planmembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Picture = table.Column<string>(maxLength: 500, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    OneTime = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Days = table.Column<int>(nullable: false),
                    AttachmentSize = table.Column<int>(nullable: false),
                    PlanCost = table.Column<int>(nullable: false),
                    CanExtend = table.Column<bool>(nullable: false),
                    DiskSpace = table.Column<long>(nullable: false),
                    Users = table.Column<int>(nullable: false),
                    WorkPackage = table.Column<int>(nullable: false),
                    Project = table.Column<int>(nullable: false),
                    SimpleGroup = table.Column<int>(nullable: false),
                    ComplexGroup = table.Column<int>(nullable: false),
                    AdditionalWorkPackageCost = table.Column<int>(nullable: false),
                    AdditionalUserCost = table.Column<int>(nullable: false),
                    AdditionalSpaceCost = table.Column<int>(nullable: false),
                    AdditionalProjectCost = table.Column<int>(nullable: false),
                    AdditionalSimpleGroupCost = table.Column<int>(nullable: false),
                    AdditionalComplexGroupCost = table.Column<int>(nullable: false),
                    FeatureCustomField = table.Column<bool>(nullable: false),
                    FeatureTimeSpent = table.Column<bool>(nullable: false),
                    FeatureTimeValue = table.Column<bool>(nullable: false),
                    FeatureTimeOff = table.Column<bool>(nullable: false),
                    FeatureShift = table.Column<bool>(nullable: false),
                    FeatureReports = table.Column<bool>(nullable: false),
                    FeaturePayments = table.Column<bool>(nullable: false),
                    FeatureChat = table.Column<bool>(nullable: false),
                    FeatureFiles = table.Column<bool>(nullable: false),
                    FeatureWbs = table.Column<bool>(nullable: false),
                    FeatureRoadMap = table.Column<bool>(nullable: false),
                    FeatureTree = table.Column<bool>(nullable: false),
                    FeatureObjectives = table.Column<bool>(nullable: false),
                    FeatureSeasons = table.Column<bool>(nullable: false),
                    FeatureVote = table.Column<bool>(nullable: false),
                    FeatureSubTask = table.Column<bool>(nullable: false),
                    FeatureKartabl = table.Column<bool>(nullable: false),
                    FeatureCalendar = table.Column<bool>(nullable: false),
                    FeatureBlocking = table.Column<bool>(nullable: false),
                    FeatureRelated = table.Column<bool>(nullable: false),
                    FeatureComplexGroup = table.Column<bool>(nullable: false),
                    FeatureGroupTimeSpent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projectmembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    IsGroup = table.Column<bool>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    Access = table.Column<byte>(nullable: false),
                    BlockNotification = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectmembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Complex = table.Column<bool>(nullable: false),
                    Premium = table.Column<bool>(nullable: false),
                    Template = table.Column<byte>(nullable: false),
                    PlanInfoId = table.Column<Guid>(nullable: false),
                    ArchivedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projectseasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectseasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 500, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    WorkingHours = table.Column<int>(nullable: false),
                    RestHours = table.Column<int>(nullable: false),
                    PenaltyRate = table.Column<float>(nullable: false),
                    RewardRate = table.Column<float>(nullable: false),
                    Start = table.Column<TimeSpan>(nullable: false),
                    End = table.Column<TimeSpan>(nullable: false),
                    Float = table.Column<TimeSpan>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subprojects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subprojects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "supportcontacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Culture = table.Column<string>(maxLength: 2, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    FullName = table.Column<string>(maxLength: 250, nullable: false),
                    Subject = table.Column<string>(maxLength: 500, nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    Seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supportcontacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "supportreplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    SupportId = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    Seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supportreplies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "testimonials",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    Culture = table.Column<string>(maxLength: 2, nullable: false),
                    Approved = table.Column<bool>(nullable: false),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testimonials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "timeoffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    BeginAt = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    EndAt = table.Column<DateTime>(nullable: false),
                    IsHourly = table.Column<bool>(nullable: false),
                    ResponderId = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeoffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "timespents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    BeginAt = table.Column<DateTime>(nullable: true),
                    EndAt = table.Column<DateTime>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: true),
                    TimeOffId = table.Column<Guid>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timespents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    ApprovedAt = table.Column<DateTime>(nullable: true),
                    TrackingCode = table.Column<string>(maxLength: 200, nullable: true),
                    ReferenceNumber = table.Column<string>(maxLength: 200, nullable: true),
                    ExternalId = table.Column<long>(maxLength: 200, nullable: true),
                    Detail = table.Column<string>(maxLength: 1500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "uploadaccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UploadId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Access = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uploadaccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "uploads",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Directory = table.Column<string>(maxLength: 2000, nullable: true),
                    Extension = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Path = table.Column<string>(maxLength: 500, nullable: false),
                    ThumbnailPath = table.Column<string>(maxLength: 500, nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    Section = table.Column<byte>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usernotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Seen = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ActivityUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usernotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userplaninfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    PlanId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 500, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Picture = table.Column<string>(maxLength: 500, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    OneTime = table.Column<bool>(nullable: false),
                    Days = table.Column<int>(nullable: false),
                    AttachmentSize = table.Column<int>(nullable: false),
                    CanExtend = table.Column<bool>(nullable: false),
                    ExpireAt = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: true),
                    PlanCost = table.Column<double>(nullable: false),
                    Space = table.Column<double>(nullable: false),
                    Users = table.Column<int>(nullable: false),
                    WorkPackage = table.Column<int>(nullable: false),
                    Project = table.Column<int>(nullable: false),
                    SimpleGroup = table.Column<int>(nullable: false),
                    ComplexGroup = table.Column<int>(nullable: false),
                    AdditionalUser = table.Column<int>(nullable: false),
                    AdditionalSpace = table.Column<double>(nullable: false),
                    AdditionalProject = table.Column<int>(nullable: false),
                    AdditionalSimpleGroup = table.Column<int>(nullable: false),
                    AdditionalComplexGroup = table.Column<int>(nullable: false),
                    AdditionalWorkPackage = table.Column<int>(nullable: false),
                    AdditionalWorkPackageCost = table.Column<int>(nullable: false),
                    AdditionalUserCost = table.Column<int>(nullable: false),
                    AdditionalSpaceCost = table.Column<int>(nullable: false),
                    AdditionalProjectCost = table.Column<int>(nullable: false),
                    AdditionalSimpleGroupCost = table.Column<int>(nullable: false),
                    AdditionalComplexGroupCost = table.Column<int>(nullable: false),
                    UsedUser = table.Column<int>(nullable: false),
                    UsedSpace = table.Column<long>(nullable: false),
                    UsedProject = table.Column<int>(nullable: false),
                    UsedSimpleGroup = table.Column<int>(nullable: false),
                    UsedComplexGroup = table.Column<int>(nullable: false),
                    UsedWorkPackage = table.Column<int>(nullable: false),
                    FeatureCustomField = table.Column<bool>(nullable: false),
                    FeatureTimeSpent = table.Column<bool>(nullable: false),
                    FeatureTimeValue = table.Column<bool>(nullable: false),
                    FeatureTimeOff = table.Column<bool>(nullable: false),
                    FeatureShift = table.Column<bool>(nullable: false),
                    FeatureReports = table.Column<bool>(nullable: false),
                    FeaturePayments = table.Column<bool>(nullable: false),
                    FeatureChat = table.Column<bool>(nullable: false),
                    FeatureFiles = table.Column<bool>(nullable: false),
                    FeatureWbs = table.Column<bool>(nullable: false),
                    FeatureRoadMap = table.Column<bool>(nullable: false),
                    FeatureTree = table.Column<bool>(nullable: false),
                    FeatureObjectives = table.Column<bool>(nullable: false),
                    FeatureSeasons = table.Column<bool>(nullable: false),
                    FeatureVote = table.Column<bool>(nullable: false),
                    FeatureSubTask = table.Column<bool>(nullable: false),
                    FeatureKartabl = table.Column<bool>(nullable: false),
                    FeatureCalendar = table.Column<bool>(nullable: false),
                    FeatureBlocking = table.Column<bool>(nullable: false),
                    FeatureRelated = table.Column<bool>(nullable: false),
                    FeatureComplexGroup = table.Column<bool>(nullable: false),
                    FeatureGroupTimeSpent = table.Column<bool>(nullable: false),
                    Yearly = table.Column<bool>(nullable: false),
                    Duration = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userplaninfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Avatar = table.Column<string>(maxLength: 500, nullable: true),
                    TimeZone = table.Column<string>(maxLength: 100, nullable: true),
                    Calendar = table.Column<int>(nullable: false),
                    Bio = table.Column<string>(maxLength: 2000, nullable: true),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    PersonalInitials = table.Column<string>(maxLength: 2, nullable: true),
                    Phone = table.Column<string>(maxLength: 50, nullable: true),
                    SkipNotification = table.Column<bool>(nullable: false),
                    DarkMode = table.Column<bool>(nullable: false),
                    Wallet = table.Column<double>(nullable: false),
                    Username = table.Column<string>(maxLength: 50, nullable: true),
                    Attempts = table.Column<int>(nullable: false),
                    Blocked = table.Column<bool>(nullable: false),
                    LastAttempt = table.Column<DateTime>(nullable: true),
                    LastEmailConfirmed = table.Column<DateTime>(nullable: true),
                    LastPhoneConfirmed = table.Column<DateTime>(nullable: true),
                    LockedUntil = table.Column<DateTime>(nullable: true),
                    MarketerId = table.Column<Guid>(nullable: true),
                    PasswordHash = table.Column<string>(maxLength: 100, nullable: true),
                    ReserveAccount = table.Column<bool>(nullable: false),
                    Salt = table.Column<string>(maxLength: 100, nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    AsanaId = table.Column<string>(maxLength: 50, nullable: true),
                    SocialId = table.Column<string>(maxLength: 50, nullable: true),
                    TaskuluId = table.Column<string>(maxLength: 50, nullable: true),
                    TaskWorldId = table.Column<string>(maxLength: 50, nullable: true),
                    TrelloId = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userverifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    ExpireAt = table.Column<DateTime>(nullable: false),
                    LastSend = table.Column<DateTime>(nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userverifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wallet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Unit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "webpushes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Android = table.Column<bool>(nullable: false),
                    Auth = table.Column<string>(maxLength: 500, nullable: false),
                    Browser = table.Column<string>(maxLength: 50, nullable: true),
                    Desktop = table.Column<bool>(nullable: false),
                    Device = table.Column<string>(maxLength: 50, nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    Endpoint = table.Column<string>(maxLength: 500, nullable: false),
                    ExpirationTime = table.Column<DateTime>(nullable: true),
                    Ios = table.Column<bool>(nullable: false),
                    Mobile = table.Column<bool>(nullable: false),
                    P256dh = table.Column<string>(maxLength: 500, nullable: false),
                    Platform = table.Column<string>(maxLength: 50, nullable: true),
                    Safari = table.Column<bool>(nullable: false),
                    Tablet = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webpushes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workingtimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    BeginAt = table.Column<DateTime>(nullable: false),
                    EndAt = table.Column<DateTime>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workingtimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagecustomfielditems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    CustomFieldId = table.Column<Guid>(nullable: false),
                    Color = table.Column<string>(maxLength: 10, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagecustomfielditems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagecustomfields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    Show = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagecustomfields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagelabels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: true),
                    Color = table.Column<string>(maxLength: 100, nullable: true),
                    DarkColor = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagelabels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagelistmembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ListId = table.Column<Guid>(nullable: false),
                    Chart = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagelistmembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagelists",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: false),
                    Color = table.Column<string>(maxLength: 100, nullable: true),
                    DarkColor = table.Column<bool>(nullable: false),
                    Kanban = table.Column<byte>(nullable: true),
                    Restricted = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ArchivedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagelists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagemembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    IsGroup = table.Column<bool>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    Access = table.Column<byte>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagemembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagemembersettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ShowTotalCards = table.Column<bool>(nullable: false),
                    ReceiveNotification = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagemembersettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackageobjectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Order = table.Column<int>(nullable: true),
                    Level = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackageobjectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagerelatedtasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    RecordPackageId = table.Column<Guid>(nullable: true),
                    RecordProjectId = table.Column<Guid>(nullable: true),
                    RecordSubProjectId = table.Column<Guid>(nullable: true),
                    RecordTaskId = table.Column<Guid>(nullable: true),
                    IsDependency = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagerelatedtasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    BeginAt = table.Column<DateTime>(nullable: true),
                    Premium = table.Column<bool>(nullable: false),
                    EndAt = table.Column<DateTime>(nullable: true),
                    ActualBeginAt = table.Column<DateTime>(nullable: true),
                    ActualEndAt = table.Column<DateTime>(nullable: true),
                    ArchivedAt = table.Column<DateTime>(nullable: true),
                    Color = table.Column<string>(maxLength: 100, nullable: true),
                    DarkColor = table.Column<bool>(nullable: false),
                    TaskVisibility = table.Column<byte>(nullable: false),
                    CommentPermission = table.Column<byte>(nullable: false),
                    AllowAttachment = table.Column<bool>(nullable: false),
                    AllowBlockingBoardTasks = table.Column<bool>(nullable: false),
                    AllowComments = table.Column<bool>(nullable: false),
                    AllowCustomField = table.Column<bool>(nullable: false),
                    AllowEndAt = table.Column<bool>(nullable: false),
                    AllowEstimatedTime = table.Column<bool>(nullable: false),
                    AllowGeoLocation = table.Column<bool>(nullable: false),
                    AllowLabels = table.Column<bool>(nullable: false),
                    AllowMembers = table.Column<bool>(nullable: false),
                    AllowPoll = table.Column<bool>(nullable: false),
                    AllowSegments = table.Column<bool>(nullable: false),
                    AllowState = table.Column<bool>(nullable: false),
                    AllowTimeSpent = table.Column<bool>(nullable: false),
                    PermissionComment = table.Column<bool>(nullable: false),
                    PermissionEditAttachment = table.Column<bool>(nullable: false),
                    PermissionCreateAttachment = table.Column<bool>(nullable: false),
                    PermissionAssignMembers = table.Column<bool>(nullable: false),
                    PermissionAssignLabels = table.Column<bool>(nullable: false),
                    PermissionChangeTaskState = table.Column<bool>(nullable: false),
                    PermissionEditTask = table.Column<bool>(nullable: false),
                    PermissionArchiveTask = table.Column<bool>(nullable: false),
                    PermissionCreateTask = table.Column<bool>(nullable: false),
                    PermissionArchiveList = table.Column<bool>(nullable: false),
                    PermissionEditList = table.Column<bool>(nullable: false),
                    PermissionCreateList = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskattachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    IsCover = table.Column<bool>(nullable: false),
                    UploadId = table.Column<Guid>(nullable: true),
                    Path = table.Column<string>(maxLength: 500, nullable: false),
                    ThumbnailPath = table.Column<string>(maxLength: 500, nullable: true),
                    Title = table.Column<string>(maxLength: 150, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskattachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskblockers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    BlockerPackageId = table.Column<Guid>(nullable: false),
                    BlockerProjectId = table.Column<Guid>(nullable: false),
                    BlockerSubProjectId = table.Column<Guid>(nullable: true),
                    BlockerId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    ReleasedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskblockers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskcomments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    Private = table.Column<bool>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ReplyId = table.Column<Guid>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskcomments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskcustomfields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CustomFieldId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    BoolValue = table.Column<bool>(nullable: true),
                    DateValue = table.Column<DateTime>(nullable: true),
                    ItemValue = table.Column<Guid>(nullable: true),
                    NumberValue = table.Column<int>(nullable: true),
                    StringValue = table.Column<string>(maxLength: 2000, nullable: true),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskcustomfields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskinteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    LastView = table.Column<DateTime>(nullable: true),
                    Watching = table.Column<bool>(nullable: true),
                    Vote = table.Column<bool>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskinteractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetasklabels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    LabelId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetasklabels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskmember",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    IsGroup = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskmember", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskobjectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    ObjectiveId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    ObjectiveValue = table.Column<byte>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskobjectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ListId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ObjectiveId = table.Column<Guid>(nullable: true),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    SeasonId = table.Column<Guid>(nullable: true),
                    CoverId = table.Column<Guid>(nullable: true),
                    DoneUserId = table.Column<Guid>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    GeoLocation = table.Column<string>(maxLength: 100, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Restricted = table.Column<bool>(nullable: false),
                    VotePaused = table.Column<bool>(nullable: false),
                    VotePrivate = table.Column<bool>(nullable: false),
                    VoteNecessity = table.Column<byte>(nullable: true),
                    BeginReminder = table.Column<byte>(nullable: true),
                    EndReminder = table.Column<byte>(nullable: true),
                    State = table.Column<byte>(nullable: false),
                    ObjectiveValue = table.Column<byte>(nullable: true),
                    EstimatedTime = table.Column<TimeSpan>(nullable: true),
                    ArchivedAt = table.Column<DateTime>(nullable: true),
                    DueAt = table.Column<DateTime>(nullable: true),
                    BeginAt = table.Column<DateTime>(nullable: true),
                    EndAt = table.Column<DateTime>(nullable: true),
                    DoneAt = table.Column<DateTime>(nullable: true),
                    LastDuePassedNotified = table.Column<DateTime>(nullable: true),
                    LastEndPassedNotified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetasktimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    Begin = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: true),
                    Manual = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetasktimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackagetaskvotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    TaskId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SubProjectId = table.Column<Guid>(nullable: true),
                    Vote = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackagetaskvotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workpackageviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: true),
                    FieldId = table.Column<Guid>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workpackageviews", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activities");

            migrationBuilder.DropTable(
                name: "blogcategories");

            migrationBuilder.DropTable(
                name: "blogposts");

            migrationBuilder.DropTable(
                name: "blogs");

            migrationBuilder.DropTable(
                name: "channels");

            migrationBuilder.DropTable(
                name: "contactreplies");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "conversationseens");

            migrationBuilder.DropTable(
                name: "discounts");

            migrationBuilder.DropTable(
                name: "errorlogs");

            migrationBuilder.DropTable(
                name: "groupmembers");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "marketerincomes");

            migrationBuilder.DropTable(
                name: "marketers");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "pendinginvitations");

            migrationBuilder.DropTable(
                name: "planmembers");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "projectmembers");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "projectseasons");

            migrationBuilder.DropTable(
                name: "shifts");

            migrationBuilder.DropTable(
                name: "subprojects");

            migrationBuilder.DropTable(
                name: "supportcontacts");

            migrationBuilder.DropTable(
                name: "supportreplies");

            migrationBuilder.DropTable(
                name: "testimonials");

            migrationBuilder.DropTable(
                name: "timeoffs");

            migrationBuilder.DropTable(
                name: "timespents");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "uploadaccesses");

            migrationBuilder.DropTable(
                name: "uploads");

            migrationBuilder.DropTable(
                name: "usernotifications");

            migrationBuilder.DropTable(
                name: "userplaninfo");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "userverifications");

            migrationBuilder.DropTable(
                name: "wallet");

            migrationBuilder.DropTable(
                name: "webpushes");

            migrationBuilder.DropTable(
                name: "workingtimes");

            migrationBuilder.DropTable(
                name: "workpackagecustomfielditems");

            migrationBuilder.DropTable(
                name: "workpackagecustomfields");

            migrationBuilder.DropTable(
                name: "workpackagelabels");

            migrationBuilder.DropTable(
                name: "workpackagelistmembers");

            migrationBuilder.DropTable(
                name: "workpackagelists");

            migrationBuilder.DropTable(
                name: "workpackagemembers");

            migrationBuilder.DropTable(
                name: "workpackagemembersettings");

            migrationBuilder.DropTable(
                name: "workpackageobjectives");

            migrationBuilder.DropTable(
                name: "workpackagerelatedtasks");

            migrationBuilder.DropTable(
                name: "workpackages");

            migrationBuilder.DropTable(
                name: "workpackagetaskattachments");

            migrationBuilder.DropTable(
                name: "workpackagetaskblockers");

            migrationBuilder.DropTable(
                name: "workpackagetaskcomments");

            migrationBuilder.DropTable(
                name: "workpackagetaskcustomfields");

            migrationBuilder.DropTable(
                name: "workpackagetaskinteractions");

            migrationBuilder.DropTable(
                name: "workpackagetasklabels");

            migrationBuilder.DropTable(
                name: "workpackagetaskmember");

            migrationBuilder.DropTable(
                name: "workpackagetaskobjectives");

            migrationBuilder.DropTable(
                name: "workpackagetasks");

            migrationBuilder.DropTable(
                name: "workpackagetasktimes");

            migrationBuilder.DropTable(
                name: "workpackagetaskvotes");

            migrationBuilder.DropTable(
                name: "workpackageviews");
        }
    }
}
