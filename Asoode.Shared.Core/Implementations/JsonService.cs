using Asoode.Shared.Abstraction.Contracts;

namespace Asoode.Shared.Core.Implementations;

internal class JsonService : IJsonService
{
    public T Deserialize<T>(string json)
    {
        return Utf8Json.JsonSerializer.Deserialize<T>(json)!;
    }

    public string Serialize(object obj)
    {
        return Utf8Json.JsonSerializer.ToJsonString(obj);
    }
}