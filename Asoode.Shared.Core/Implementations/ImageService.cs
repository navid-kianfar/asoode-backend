using Asoode.Shared.Abstraction.Contracts;
using SkiaSharp;

namespace Asoode.Shared.Core.Implementations;

internal class ImageService : IImageService
{
    private readonly ILoggerService _loggerService;

    public ImageService(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    public async Task<MemoryStream?> Resize(Stream input, int maxWidth, int maxHeight)
    {
        try
        {
            using var sourceBitmap = SKBitmap.Decode(input);
            var height = Math.Min(maxHeight, sourceBitmap.Height);
            var width = Math.Min(maxWidth, sourceBitmap.Width);
            using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
            using var scaledImage = SKImage.FromBitmap(scaledBitmap);
            using var data = scaledImage.Encode();
            var result = new MemoryStream(data.ToArray());
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ImageService.Resize", e);
            return null;
        }
    }

    public Task<MemoryStream?> Resize(MemoryStream input, int width, int height)
    {
        return Resize(input, width, height);
    }
}