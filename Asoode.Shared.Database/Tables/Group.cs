using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Group : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    public Guid PlanInfoId { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    public GroupType Type { get; set; }
    public DateTime? ExpireAt { get; set; }
    [MaxLength(500)] public string Avatar { get; set; }
    public Guid? ParentId { get; set; }
    public Guid RootId { get; set; }
    public int Level { get; set; }

    public bool Premium { get; set; }
    public bool Complex { get; set; }

    #region Properties

    public DateTime? ArchivedAt { get; set; }

    public DateTime? RegisteredAt { get; set; }
    [MaxLength(2000)] public string SubTitle { get; set; }
    [MaxLength(2000)] public string BrandTitle { get; set; }
    [MaxLength(250)] public string SupervisorName { get; set; }
    [MaxLength(50)] public string SupervisorNumber { get; set; }
    [MaxLength(250)] public string ResponsibleName { get; set; }
    [MaxLength(50)] public string ResponsibleNumber { get; set; }
    [MaxLength(250)] public string Email { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    [MaxLength(500)] public string Website { get; set; }
    [MaxLength(100)] public string PostalCode { get; set; }
    [MaxLength(2000)] public string Address { get; set; }
    [MaxLength(50)] public string Tel { get; set; }
    [MaxLength(50)] public string Fax { get; set; }
    [MaxLength(100)] public string GeoLocation { get; set; }
    [MaxLength(100)] public string NationalId { get; set; }
    [MaxLength(100)] public string RegistrationId { get; set; }
    public int? Offices { get; set; }
    public int? Employees { get; set; }

    #endregion

    public GroupDto ToDto()
    {
        return new GroupDto
        {
            Type = Type,
            Address = Address,
            Avatar = Avatar,
            Complex = Complex,
            Description = Description,
            Employees = Employees,
            Email = Email,
            Fax = Fax,
            Level = Level,
            ArchivedAt = ArchivedAt,
            Offices = Offices,
            BrandTitle = BrandTitle,
            Title = Title,
            Website = Website,
            GeoLocation = GeoLocation,
            NationalId = NationalId,
            Premium = Premium, 
            ExpireAt = ExpireAt,
            PostalCode = PostalCode,
            RegisteredAt = RegisteredAt,
            RegistrationId = RegistrationId,
            ResponsibleName = ResponsibleName,
            ParentId = ParentId,
            ResponsibleNumber = ResponsibleNumber,
            SupervisorName = SupervisorName,
            SupervisorNumber = SupervisorNumber,
            Tel = Tel,
            SubTitle = SubTitle,
            RootId = RootId,
            UserId = UserId,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            // PlanType = PlanType,
            // Members = Members,
            // Pending = Pending,
            // AttachmentSize = AttachmentSize,
        };
    }
}