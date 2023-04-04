namespace Asoode.Application.Core.Contracts.General;

public interface IJsonService
{
    T Deserialize<T>(string json);

    string Serialize(object obj);
}