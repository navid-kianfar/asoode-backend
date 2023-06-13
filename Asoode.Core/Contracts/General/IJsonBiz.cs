namespace Asoode.Core.Contracts.General;

public interface IJsonBiz
{
    T Deserialize<T>(string json);

    string Serialize(object obj);
}