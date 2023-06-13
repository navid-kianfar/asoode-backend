using System;
using System.Threading.Tasks;

namespace Asoode.Core.Contracts.General;

public interface ILoggerService
{
    Task Info(string message, string section);

    Task Log(string section, string message);
    Task Debug(string message, string section);
    Task Error(string message, string section, Exception ex = null);
    
    Task Error(Exception ex);
}