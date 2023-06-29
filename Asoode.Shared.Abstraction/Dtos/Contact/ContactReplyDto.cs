namespace Asoode.Shared.Abstraction.Dtos.Contact;

public record ContactReplyDto : BaseDto
{
    public Guid ContactId { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; }
}