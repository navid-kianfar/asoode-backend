namespace Asoode.Shared.Abstraction.Dtos.General;

public record ErrorLogPayload
{
    public Guid UserId { get; set; }
    public Guid RecordId { get; set; }
    public dynamic Data { get; set; }
}