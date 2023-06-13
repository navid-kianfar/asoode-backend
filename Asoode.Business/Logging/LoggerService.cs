using System;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;

namespace Asoode.Business.Logging;

internal class ConsoleLogger : ILoggerService
{
    private const string LogLineSeparator =
        "-----------------------------------------------------------------------------------";
    
    public Task Log(string message, string section)
    {
        return HandleError(ConsoleColor.DarkYellow, message, section);
    }

    public Task Info(string message, string section)
    {
        return HandleError(ConsoleColor.Blue, message, section);
    }

    public Task Debug(string message, string section)
    {
        return HandleError(ConsoleColor.Magenta, message, section);
    }

    public Task Error(string message, string section, Exception? ex = null)
    {
        return HandleError(ConsoleColor.Red, ex?.Message ?? message, section);
    }

    public Task Error(Exception ex)
        => Error(ex.Message, ex.Source, ex);

    private Task HandleError(ConsoleColor color, string message, string section)
    {
        Console.ResetColor();
        Console.WriteLine(LogLineSeparator);
        Console.ForegroundColor = color;
        Console.WriteLine(section);
        Console.ResetColor();
        Console.WriteLine(message);
        Console.WriteLine(LogLineSeparator);
        return Task.CompletedTask;
    }
}