using Asoode.Shared.Abstraction.Dtos;

namespace Asoode.Admin.Abstraction.Dtos;

public record ContactReplyDto : BaseDto
{
    public Guid ContactId { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; }
}