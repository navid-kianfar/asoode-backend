using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class WorkPackageMemberSetting : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public Guid ProjectId { get; set; }
        public bool ShowTotalCards { get; set; }
        public ReceiveNotificationType ReceiveNotification { get; set; }
    }
}