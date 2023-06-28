namespace Asoode.Shared.Abstraction.Contracts;

public interface IHttpService
{
    Task<string?> Submit(string path, object? payload);
    Task<T?> Post<T>(string path, object? payload);
    Task<T?> Get<T>(string path);
    Task<T?> Get<T>(string path, Dictionary<string, string> parameters);
    void ConfigureEndpoint(string endpoint);

    void ConfigureToken(string token, string? schema = null);
}