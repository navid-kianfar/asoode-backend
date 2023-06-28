namespace Asoode.Shared.Abstraction.Contracts;

public interface IImageService
{
    public Task<MemoryStream?> Resize(Stream input, int width, int height);
    public Task<MemoryStream?> Resize(MemoryStream input, int width, int height);
}