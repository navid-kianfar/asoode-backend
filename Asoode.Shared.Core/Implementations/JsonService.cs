using Asoode.Shared.Abstraction.Contracts;
using Utf8Json;

namespace Asoode.Shared.Core.Implementations;

internal class JsonService : IJsonService
{
    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }

    public string Serialize(object obj)
    {
        return JsonSerializer.ToJsonString(obj);
    }
}