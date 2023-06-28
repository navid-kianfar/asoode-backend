namespace Asoode.Shared.Abstraction.Fixtures;

public record SharedConstants
{
    public const string PublicBucket = "Public";
    public const string ProtectedBucket = "Protected";
    public const string UploadPublic = "storage/upload/public";
    public const string UploadProtected = "storage/upload/protected";
    public const string DownloadPublic = "storage/download/public/{*path}";
    public const string DownloadProtected = "storage/download/protected/{*path}";
}