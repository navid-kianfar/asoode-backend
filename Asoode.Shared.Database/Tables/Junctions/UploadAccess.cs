using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class UploadAccess : BaseEntity
{
    public Guid UploadId { get; set; }
    public Guid UserId { get; set; }
    public AccessType Access { get; set; }
}