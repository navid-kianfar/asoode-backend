using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class ErrorLog : BaseEntity
{
    [Required] [MaxLength(4000)] public string Description { get; set; }
    [Required] [MaxLength(8000)] public string ErrorBody { get; set; }
}