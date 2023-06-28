using Asoode.Shared.Abstraction.Dtos;

namespace Asoode.Application.Server.Extensions;

public static class FileExtensions
{
    public static async Task<StorageItemDto> ToStorageItem(this IFormFile file)
    {
        var storageItem = new StorageItemDto
        {
            Stream = new MemoryStream(),
            Extension = Path.GetExtension(file.FileName),
            CreatedAt = DateTime.UtcNow,
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            Url = string.Empty
        };
        await file.CopyToAsync(storageItem.Stream);
        storageItem.Stream.Seek(0, SeekOrigin.Begin);

        return storageItem;
    }
}