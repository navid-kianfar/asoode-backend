using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Contracts;

public interface ICacheService
{
    string GetKey(CacheStorageType section, string id);
    string GetKey(CacheStorageType section, object payload, string? id = null);

    Task<bool> Store(CacheStorageType section, object payload, string? id = null, TimeSpan? expiresIn = null);
    Task<bool> Store(string cacheKey, object payload, TimeSpan? expiresIn = null);
    Task<T?> Retrieve<T>(CacheStorageType section, object payload, string? id = null);
    Task<T?> Retrieve<T>(string cacheKey);
}