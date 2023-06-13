using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class Group : BaseEntity
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
}