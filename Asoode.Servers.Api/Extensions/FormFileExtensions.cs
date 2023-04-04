using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Servers.Api.Extensions;

public static class FormFileExtensions
{
    public static async Task<UploadedFileViewModel?> ToViewModel(this IFormFile? file)
    {
        if (file == null) return null;
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return new UploadedFileViewModel
        {
            Size = file.Length,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileStream = stream
        };
    }
}