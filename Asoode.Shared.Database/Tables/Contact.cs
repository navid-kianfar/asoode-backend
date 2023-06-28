using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class Contact : BaseEntity
{
    [Required] [MaxLength(100)] public string FirstName { get; set; }
    [Required] [MaxLength(100)] public string LastName { get; set; }
    [Required] [MaxLength(200)] public string Email { get; set; }
    [Required] [MaxLength(1000)] public string Message { get; set; }
    public bool Seen { get; set; }
}