using System.Web;
using Asoode.Shared.Abstraction.Dtos;

namespace Asoode.Admin.Server.Extensions;

public static class FileExtensions
{
    public static async Task<StorageItemDto[]> ToStorageItems(this IFormFileCollection files)
    {
        var items = new List<StorageItemDto>();
        foreach (var file in files)
            items.Add(await file.ToStorageItem());
        return items.ToArray();
    }

    public static async Task<StorageItemDto> ToStorageItem(this IFormFile file)
    {
        var storageItem = new StorageItemDto
        {
            Stream = new MemoryStream(),
            Extension = Path.GetExtension(file.FileName),
            CreatedAt = DateTime.UtcNow,
            FileName = HttpUtility.UrlEncode(file.FileName),
            FileSize = file.Length,
            MimeType = file.ContentType,
            FileField = file.Name,
            Url = string.Empty
        };

        await file.CopyToAsync(storageItem.Stream);
        storageItem.Stream.Seek(0, SeekOrigin.Begin);

        return storageItem;
    }
}