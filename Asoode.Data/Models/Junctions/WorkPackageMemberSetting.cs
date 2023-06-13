using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class WorkPackageMemberSetting : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public bool ShowTotalCards { get; set; }
    public ReceiveNotificationType ReceiveNotification { get; set; }
}