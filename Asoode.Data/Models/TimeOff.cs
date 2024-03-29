using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class TimeOff : BaseEntity
{
    public DateTime BeginAt { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsHourly { get; set; }
    public Guid ResponderId { get; set; }
    public RequestStatus Status { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}