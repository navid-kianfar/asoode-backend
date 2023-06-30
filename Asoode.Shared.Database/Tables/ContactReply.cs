using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class ContactReply : BaseEntity
{
    public Guid ContactId { get; set; }
    public Guid UserId { get; set; }
    [Required] [MaxLength(2000)] public string Message { get; set; }

    public ContactReplyDto ToDto()
    {
        return new ContactReplyDto
        {
            Message = Message,
            ContactId = ContactId,
            Id = Id,
            CreatedAt = CreatedAt,
            UserId = UserId,
            UpdatedAt = UpdatedAt
        };
    }
}