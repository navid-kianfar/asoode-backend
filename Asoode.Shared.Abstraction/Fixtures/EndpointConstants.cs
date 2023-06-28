namespace Asoode.Shared.Abstraction.Fixtures;

public static class EndpointConstants
{
    public const string Prefix = "api/v1/zaren-travel";
    public const string WebhookPrefix = "webhook";
    public const string UploadPublic = "storage/upload/public";
    public const string UploadProtected = "storage/upload/protected";
    public const string DownloadPublic = "storage/download/public/{*path}";
    public const string DownloadProtected = "storage/download/protected/{*path}";

}