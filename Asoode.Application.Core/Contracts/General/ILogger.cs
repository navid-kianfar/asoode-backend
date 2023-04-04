namespace Asoode.Application.Core.Contracts.General;

public interface ILoggerService
{
    Task Error(string message, Exception ex);
    Task Exception(Exception ex);

    Task Log(string section, string message);
}