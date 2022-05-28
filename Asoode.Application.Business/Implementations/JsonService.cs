using Asoode.Application.Core.Contracts;

namespace Asoode.Application.Business.Implementations;

internal class JsonService : IJsonService
{
    public T Deserialize<T>(string json)
    {
        throw new NotImplementedException();
    }

    public string Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}