using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Core.Helpers;
using StackExchange.Redis;

namespace Asoode.Shared.Core.Implementations;

internal record CacheService : ICacheService
{
    private readonly IDatabase _cache;
    private readonly IJsonService _jsonService;

    public CacheService(IJsonService jsonService)
    {
        _jsonService = jsonService;
        _cache = RedisHandler.Connection.GetDatabase();
    }

    public string GetKey(CacheStorageType section, string id)
    {
        return CryptographyHelper.ComputeSHA256($"{section.ToString()}-{id}");
    }

    public string GetKey(CacheStorageType section, object model, string? id = null)
    {
        var serialized = _jsonService.Serialize(model).ToLower();
        return CryptographyHelper.ComputeSHA256($"{section.ToString()}-{serialized}-{id}");
    }

    public Task<bool> Store(CacheStorageType section, object payload, string? id = null, TimeSpan? expiresIn = null)
    {
        var key = GetKey(section, payload, id);
        return Store(key, payload, expiresIn);
    }

    public Task<T?> Retrieve<T>(CacheStorageType section, object payload, string? id = null)
    {
        var key = GetKey(section, payload, id);
        return Retrieve<T>(key);
    }

    public async Task<bool> Store(string cacheKey, object payload, TimeSpan? expiresIn = null)
    {
        var key = new RedisKey(cacheKey);
        var value = new RedisValue(_jsonService.Serialize(payload));
        return await _cache.StringSetAsync(key, value, expiresIn);
    }

    public async Task<T?> Retrieve<T>(string cacheKey)
    {
        var key = new RedisKey(cacheKey);

        var exists = await _cache.KeyExistsAsync(key);
        if (!exists) return default;

        var data = await _cache.StringGetAsync(key);
        return data.HasValue ? _jsonService.Deserialize<T>(data.ToString()) : default;
    }
}