using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class WorkPackageMemberSetting : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public bool ShowTotalCards { get; set; }
    public ReceiveNotificationType ReceiveNotification { get; set; }
}