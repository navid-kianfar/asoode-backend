namespace Asoode.Application.Core.Primitives.Exceptions;

public class InvalidDateException : Exception
{
    public InvalidDateException() :
        base("Provided date is not valid")
    {
    }

    public InvalidDateException(string date) :
        base($"Provided date : '{date}' is not valid")
    {
    }
}