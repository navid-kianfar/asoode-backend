using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Contact : BaseEntity
{
    [Required] [MaxLength(100)] public string FirstName { get; set; }
    [Required] [MaxLength(100)] public string LastName { get; set; }
    [Required] [MaxLength(200)] public string Email { get; set; }
    [Required] [MaxLength(1000)] public string Message { get; set; }
    public bool Seen { get; set; }

    public ContactListDto ToDto(int index)
    {
        return new ContactListDto
        {
            Email = Email,
            Message = Message,
            FirstName = FirstName,
            LastName = LastName,
            Index = index,
            Seen = Seen,
            CreatedAt = CreatedAt,
            Id = Id
        };
    }
}