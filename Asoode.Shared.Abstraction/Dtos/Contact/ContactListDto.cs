namespace Asoode.Shared.Abstraction.Dtos.Contact;

public class ContactListDto : ContactDto
{
    public bool Seen { get; set; }
    public int Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
}