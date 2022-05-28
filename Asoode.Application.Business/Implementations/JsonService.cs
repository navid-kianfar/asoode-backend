using Asoode.Application.Core.Contracts;
using Utf8Json;

namespace Asoode.Application.Business.Implementations;

internal class JsonService : IJsonService
{
    public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json);

    public string Serialize(object obj) => JsonSerializer.ToJsonString(obj);
}