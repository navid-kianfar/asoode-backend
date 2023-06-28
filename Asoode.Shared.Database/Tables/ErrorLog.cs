using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class ErrorLog : BaseEntity
{
    [Required] [MaxLength(4000)] public string Description { get; set; }
    [Required] [MaxLength(8000)] public string ErrorBody { get; set; }
}