namespace Asoode.Shared.Abstraction.Contracts;

public interface ILoggerService
{
    Task Log(string message, string section);
    Task Info(string message, string section);
    Task Debug(string message, string section);
    Task Error(string message, string section, Exception? ex = null);
}