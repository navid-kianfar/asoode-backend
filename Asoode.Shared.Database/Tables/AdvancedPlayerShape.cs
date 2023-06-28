using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class AdvancedPlayerShape : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid AttachmentId { get; set; }
    public int StartFrame { get; set; }
    public int EndFrame { get; set; }
}