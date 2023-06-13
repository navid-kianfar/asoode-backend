using System;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class AdvancedPlayerShape : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid AttachmentId { get; set; }
    public int StartFrame { get; set; }
    public int EndFrame { get; set; }
}