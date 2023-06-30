using Asoode.Shared.Abstraction.Dtos.Storage;

namespace Asoode.Shared.Abstraction.Dtos.Postman;

public record SendMailDTO
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public StorageItemDto[] Attachments { get; set; } = Array.Empty<StorageItemDto>();
}