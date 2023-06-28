using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class WorkPackageTaskComment : BaseEntity
{
    [Required] [MaxLength(1000)] public string Message { get; set; }
    public bool Private { get; set; }
    [Required] public Guid TaskId { get; set; }
    [Required] public Guid UserId { get; set; }
    public Guid? ReplyId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
}