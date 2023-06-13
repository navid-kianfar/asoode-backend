using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class Blog : BaseEntity
{
    public BlogType Type { get; set; }
    [Required] [MaxLength(2)] public string Culture { get; set; }
    [Required] [MaxLength(1000)] public string Keywords { get; set; }
    [Required] [MaxLength(2000)] public string Description { get; set; }
    [Required] [MaxLength(1000)] public string Title { get; set; }
}