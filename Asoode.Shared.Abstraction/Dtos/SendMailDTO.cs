namespace Asoode.Shared.Abstraction.Dtos;

public record SendMailDTO
{
    public string To { get; set; } = String.Empty;
    public string Subject { get; set; } = String.Empty;
    public string Message { get; set; } = String.Empty;
    public StorageItemDto[] Attachments { get; set; } = Array.Empty<StorageItemDto>();
}