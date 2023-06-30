namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record BulkDownloadResultDto
{
    public Stream Zip { get; set; }
    public string Title { get; set; }
}