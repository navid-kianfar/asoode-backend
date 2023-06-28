using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class Testimonial : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    [Required] [MaxLength(1000)] public string Message { get; set; }
    [Required] [MaxLength(2)] public string Culture { get; set; }
    public bool Approved { get; set; }
    public int Rate { get; set; }
}