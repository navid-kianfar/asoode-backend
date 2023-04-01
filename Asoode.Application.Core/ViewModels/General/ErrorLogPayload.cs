namespace Asoode.Application.Core.ViewModels.General;

public class ErrorLogPayload
{
    public Guid UserId { get; set; }
    public Guid RecordId { get; set; }
    public dynamic Data { get; set; }
}