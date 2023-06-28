namespace Asoode.Shared.Abstraction.Contracts;

public interface IJsonService
{
    T Deserialize<T>(string json);

    string Serialize(object obj);
}