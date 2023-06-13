using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class UploadAccess : BaseEntity
{
    public Guid UploadId { get; set; }
    public Guid UserId { get; set; }
    public AccessType Access { get; set; }
}