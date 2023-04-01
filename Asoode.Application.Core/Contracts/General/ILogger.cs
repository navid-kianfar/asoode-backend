namespace Asoode.Application.Core.Contracts.General;

public interface ILogger
{
    Task Exception(Exception ex);

    Task Log(string section, string message);
}